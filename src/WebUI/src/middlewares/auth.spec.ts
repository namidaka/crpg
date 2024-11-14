import { createTestingPinia } from '@pinia/testing'
import { ErrorResponse } from 'oidc-client-ts'

import { getRoute, next } from '~/__mocks__/router'
import Role from '~/models/role'
import { useUserStore } from '~/stores/user'

import { authRouterMiddleware, signInCallback } from './auth'

const { mockedGetUser, mockedSignInCallback } = vi.hoisted(() => ({
  mockedGetUser: vi.fn(),
  mockedSignInCallback: vi.fn(),
}))

vi.mock('~/services/auth-service', () => {
  return {
    getUser: mockedGetUser,
    userManager: {
      signinCallback: mockedSignInCallback,
    },
  }
})

const userStore = useUserStore(createTestingPinia())

beforeEach(() => {
  userStore.$reset()
})

const from = getRoute()

it('skip route validation', async () => {
  const to = getRoute({
    meta: {
      layout: 'default',
      skipAuth: true,
    },
    // @ts-expect-error TODO:
    name: 'skip-auth-route',
    path: '/skip-auth-route',
  })

  expect(await authRouterMiddleware(to, from, next)).toEqual(true)

  expect(userStore.fetchUser).not.toBeCalled()
})

describe('route not requires any role', () => {
  const to = getRoute({
    // @ts-expect-error TODO:
    name: 'user',
    path: '/user',
  })

  it('!userStore && !isSignIn', async () => {
    mockedGetUser.mockResolvedValueOnce(null)

    expect(await authRouterMiddleware(to, from, next)).toEqual(true)

    expect(userStore.fetchUser).not.toBeCalled()
    expect(mockedGetUser).toBeCalled()
  })

  it('!userStore && isSignIn', async () => {
    mockedGetUser.mockResolvedValueOnce({})

    expect(await authRouterMiddleware(to, from, next)).toEqual(true)

    expect(userStore.fetchUser).toBeCalled()
    expect(mockedGetUser).toBeCalled()
  })
})

describe('route requires role', () => {
  const to = getRoute({
    meta: {
      roles: ['User'],
    },
    // @ts-expect-error TODO:
    name: 'user',
    path: '/user',
  })

  it('!user + !isSignIn -> go to index page', async () => {
    expect(await authRouterMiddleware(to, from, next)).toEqual({ name: 'Root' })
    expect(userStore.fetchUser).toBeCalled()
    expect(mockedGetUser).toBeCalled()
  })

  it('user with role:User -> validation passed', async () => {
    userStore.$patch({ user: { role: Role.User } })

    expect(await authRouterMiddleware(to, from, next)).toEqual(true)
    expect(userStore.fetchUser).not.toBeCalled()
    expect(mockedGetUser).not.toBeCalled()
  })
})

describe('with Admin or Moderator role', () => {
  const to = getRoute({
    meta: {
      roles: ['Admin', 'Moderator'],
    },
    // @ts-expect-error TODO:
    name: 'admin',
    path: '/admin',
  })

  it('user with role:User -> go to index page', async () => {
    userStore.$patch({ user: { role: Role.User } })

    expect(await authRouterMiddleware(to, from, next)).toEqual({ name: 'Root' })
  })

  it('user with role:Admin -> validation passed', async () => {
    userStore.$patch({ user: { role: Role.Admin } })

    expect(await authRouterMiddleware(to, from, next)).toEqual(true)
  })

  it('user with role:Moderator -> validation passed', async () => {
    userStore.$patch({ user: { role: Role.Moderator } })

    expect(await authRouterMiddleware(to, from, next)).toEqual(true)
  })
})

describe('signInCallback', () => {
  it('ok', async () => {
    expect(await signInCallback(getRoute(), getRoute(), next)).toEqual({ name: 'Characters' })
    expect(mockedSignInCallback).toHaveBeenCalled()
  })

  it('error - invalid grant', async () => {
    mockedSignInCallback.mockRejectedValue(
      new ErrorResponse({
        error: 'access_denied',
      }),
    )

    expect(await signInCallback(getRoute(), getRoute(), next)).toEqual({ name: 'Banned' })
  })

  it('error - invalid grant TODO: change title', async () => {
    mockedSignInCallback.mockRejectedValue({ error: 'some error' })

    expect(await signInCallback(getRoute(), getRoute(), next)).toEqual({ name: 'Root' })
  })
})
