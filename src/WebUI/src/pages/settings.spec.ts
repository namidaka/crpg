import { createTestingPinia } from '@pinia/testing'

import type { Character } from '~/models/character'
import type { User } from '~/models/user'

import { mountWithRouter } from '~/__test__/unit/utils'
import { Region } from '~/models/region'
import { useUserStore } from '~/stores/user'

import Page from './settings.vue'

const { mockedDeleteUser, mockedHide, mockedLogout, mockedNotify } = vi.hoisted(() => ({
  mockedDeleteUser: vi.fn(),
  mockedHide: vi.fn(),
  mockedLogout: vi.fn(),
  mockedNotify: vi.fn(),
}))
vi.mock('~/services/notification-service', () => ({
  notify: mockedNotify,
}))
vi.mock('~/services/users-service', () => ({
  deleteUser: mockedDeleteUser,
}))
vi.mock('~/services/auth-service', () => ({
  logout: mockedLogout,
}))

const userStore = useUserStore(createTestingPinia())
userStore.$patch({ user: { region: Region.Eu } as User })

const routes = [
  {
    component: Page,
    name: 'settings',
    path: '/settings',
  },
]
const route = {
  name: 'settings',
}
const mountOptions = {
  global: {
    stubs: {
      'FormGroup': {
        template: `<div data-aq-FormGroup-stub>
          <slot />
        </div>`,
      },
      'i18n-t': {
        template: `<div data-aq-i18n-t-stub>
                    <slot name="link"/>
                  </div>`,
      },
      'Modal': {
        setup() {
          return {
            hide: mockedHide,
          }
        },
        template: `<div data-aq-modal-stub>
                    <slot v-bind="{ hide }" />
                    <slot name="popper" v-bind="{ hide }" />
                  </div>`,
      },
    },
  },
}

it('can delete user', async () => {
  const USER_NAME = 'Fluttershy'
  userStore.user = { name: USER_NAME } as User

  // @ts-expect-error TODO:
  const { wrapper } = await mountWithRouter(mountOptions, routes, route)

  expect(wrapper.find('[data-aq-cant-delete-user-message]').exists()).toBeFalsy()
  expect(wrapper.find('[data-aq-delete-user-group]').classes()).not.toContain(
    'pointer-events-none',
  )
  expect(wrapper.find('[data-aq-modal-stub]').attributes().disabled).toEqual('false')

  const ConfirmActionForm = wrapper.findComponent({ name: 'ConfirmActionForm' })

  expect(ConfirmActionForm.props('name')).toEqual(
    `user.settings.delete.dialog.enterToConfirm::user:${USER_NAME}`,
  )

  await ConfirmActionForm.vm.$emit('confirm')

  expect(mockedDeleteUser).toBeCalled()
  expect(mockedNotify).toBeCalledWith('user.settings.delete.notify.success')
  expect(mockedLogout).toBeCalled()
  expect(mockedHide).toBeCalled()
})

it('can`t delete user', async () => {
  const USER_NAME = 'Fluttershy'
  userStore.user = { name: USER_NAME } as User
  userStore.characters = [{ id: 1 }] as Character[]

  // @ts-expect-error TODO:
  const { wrapper } = await mountWithRouter(mountOptions, routes, route)

  expect(wrapper.find('[data-aq-cant-delete-user-message]').exists()).toBeTruthy()
  expect(wrapper.find('[data-aq-delete-user-group]').classes()).toContain('pointer-events-none')
  expect(wrapper.find('[data-aq-modal-stub]').attributes().disabled).toEqual('true')
})
