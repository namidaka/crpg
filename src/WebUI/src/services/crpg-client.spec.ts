import { mockDelete, mockGet, mockPost, mockPut } from 'vi-fetch'

import { ErrorType, type Result } from '~/models/crpg-client-result'

import { del, get, JSONDateToJs, post, put } from './crpg-client'

const { mockedGetToken, mockedLogin, mockedNotify, mockedSleep } = vi.hoisted(() => ({
  mockedGetToken: vi.fn(),
  mockedLogin: vi.fn(),
  mockedNotify: vi.fn(),
  mockedSleep: vi.fn().mockResolvedValue(null),
}))

vi.mock('~/services/auth-service', () => ({
  getToken: mockedGetToken,
  login: mockedLogin,
}))

vi.mock('~/utils/promise', () => ({
  sleep: mockedSleep,
}))

vi.mock('~/services/notification-service', async () => {
  return {
    ...(await vi.importActual<typeof import('~/services/notification-service')>(
      '~/services/notification-service',
    )),
    notify: mockedNotify,
  }
})

describe('get', () => {
  const path = '/test-get'

  it('oK 200', async () => {
    const response: Result<any> = {
      data: {
        description: 'The best',
        name: 'Rainbow Dash',
      },
      errors: null,
    }

    mockGet(path).willResolve(response, 200)

    const result = await get(path)

    expect(result).toEqual(response.data)
  })

  it('with error - InternalError', async () => {
    const response: Result<any> = {
      data: null,
      errors: [
        {
          code: '500',
          detail: null,
          stackTrace: null,
          title: 'some error',
          traceId: null,
          type: ErrorType.InternalError,
        },
      ],
    }

    mockGet(path).willResolve(response, 200)

    // ref https://vitest.dev/api/#rejects
    await expect(get(path)).rejects.toThrow('Server error')

    expect(mockedNotify).toBeCalledWith('some error', 'danger')
  })

  it('with error - other', async () => {
    const response: Result<any> = {
      data: null,
      errors: [
        {
          code: '500',
          detail: null,
          stackTrace: null,
          title: 'some warning',
          traceId: null,
          type: ErrorType.Forbidden,
        },
      ],
    }

    mockGet(path).willResolve(response, 200)

    await expect(get(path)).rejects.toThrow('Bad request')

    expect(mockedNotify).toBeCalledWith('some warning', 'warning')
  })

  it('unauthorized', async () => {
    mockGet(path).willFail({}, 401)

    const result = await get(path)
    expect(result).toEqual(null)

    expect(mockedNotify).toBeCalledWith('Session expired', 'warning')
    expect(mockedSleep).toBeCalled()
  })
})

describe('post', () => {
  const path = '/test-post'

  it('oK 204 NO_CONTENT', async () => {
    const response: Result<any> = {
      data: {},
      errors: null,
    }

    mockPost(path).willResolve(response, 204)

    const result = await post(path, {
      description: 'The best',
      name: 'Rainbow Dash',
    })

    expect(result).toEqual(null)
  })
})

describe('put', () => {
  const path = '/test-put/1'

  it('oK 204', async () => {
    const response: Result<any> = {
      data: {},
      errors: null,
    }

    mockPut(path).willResolve(response, 204)

    const result = await put(path, {
      description: 'The best',
      name: 'Rainbow Dash',
    })

    expect(result).toEqual(null)
  })

  describe('del', () => {
    const path = '/test-del/1'

    it('oK 204', async () => {
      const response: Result<any> = {
        data: {},
        errors: null,
      }

      mockDelete(path).willResolve(response, 204)

      const result = await del(path)

      expect(result).toEqual(null)
    })
  })
})

it('jSONDateToJs', () => {
  expect(JSONDateToJs('2023-11-17T18:50:13.659473Z')).toEqual(new Date('2023-11-17T18:50:13.659Z'))

  expect(
    JSONDateToJs({
      createdAt: '2023-11-17T18:50:13.659473Z',
    }),
  ).toEqual({ createdAt: new Date('2023-11-17T18:50:13.659Z') })

  expect(
    JSONDateToJs({
      id: 1,
      nested: {
        createdAt: '2023-11-17T18:50:13.659473Z',
      },
    }),
  ).toEqual({
    id: 1,
    nested: {
      createdAt: new Date('2023-11-17T18:50:13.659473Z'),
    },
  })
})
