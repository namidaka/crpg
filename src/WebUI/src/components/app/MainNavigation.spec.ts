import { createTestingPinia } from '@pinia/testing'
import { mount } from '@vue/test-utils'

import Role from '~/models/role'
import { useUserStore } from '~/stores/user'

import MainNavigation from './MainNavigation.vue'

const userStore = useUserStore(createTestingPinia())

const getWrapper = () =>
  mount(MainNavigation, {
    global: {
      stubs: ['RouterLink', 'VTooltip'],
    },
  })

describe('mod link', () => {
  it('role:user - not exist', () => {
    userStore.$patch({ user: { role: Role.User } })

    const wrapper = getWrapper()

    expect(wrapper.find('[data-aq-main-nav-link="Moderator"]').exists()).toBeFalsy()
  })

  it('role:mod', () => {
    userStore.$patch({ user: { role: Role.Moderator } })

    const wrapper = getWrapper()

    expect(wrapper.find('[data-aq-main-nav-link="Moderator"]').exists()).toBeTruthy()
  })
})

describe('clan explanation', () => {
  it('no clan - exist', () => {
    userStore.$patch({ clan: null, user: { role: Role.User } })

    const wrapper = getWrapper()

    expect(wrapper.find('[data-aq-main-nav-link-tooltip="Explanation"]').exists()).toBeTruthy()
  })

  it('with clan - exist', () => {
    userStore.$patch({ clan: { id: 1 }, user: { role: Role.User } })

    const wrapper = getWrapper()

    expect(wrapper.find('[data-aq-main-nav-link-tooltip="Explanation"]').exists()).toBeFalsy()
  })
})
