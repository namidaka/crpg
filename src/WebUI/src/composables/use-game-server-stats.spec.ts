import { mount } from '@vue/test-utils'

import { useGameServerStats } from './use-game-server-stats'

const { mockedGetGameServerStats, mockedSubscribe, mockedUnsubscribe } = vi.hoisted(() => ({
  mockedGetGameServerStats: vi.fn(),
  mockedSubscribe: vi.fn(),
  mockedUnsubscribe: vi.fn(),
}))
const { mockUsePollInterval } = vi.hoisted(() => ({
  mockUsePollInterval: vi.fn().mockImplementation(() => ({
    subscribe: mockedSubscribe,
    unsubscribe: mockedUnsubscribe,
  })),
}))
vi.mock('~/composables/use-poll-interval', () => ({
  usePollInterval: mockUsePollInterval,
}))
vi.mock('~/service/game-server-statistics-service', () => ({
  getGameServerStats: mockedGetGameServerStats,
}))

it('useGameServerStats composable lifecycle', async () => {
  const TestComponent = defineComponent({
    setup() {
      return {
        ...useGameServerStats(),
      }
    },
    template: '<div/>',
  })

  const wrapper = mount(TestComponent)

  expect(mockedSubscribe).toBeCalled()
  expect(mockedGetGameServerStats).not.toBeCalled()

  wrapper.unmount()
  expect(mockedUnsubscribe).toBeCalled()
})
