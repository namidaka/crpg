import { createTestingPinia } from '@pinia/testing'

import { Region } from '~/models/region'
import { useUserStore } from '~/stores/user'

import { useHappyHours } from './use-hh'

const { mockedGetHHEventByRegion, mockedGetHHEventRemaining, mockedNotify } = vi.hoisted(() => ({
  mockedGetHHEventByRegion: vi.fn().mockReturnValue({
    end: new Date('2000-02-02T00:00:00.000Z'),
    start: new Date('2000-02-01T00:00:00.000Z'),
  }),
  mockedGetHHEventRemaining: vi.fn().mockReturnValue(1000),
  mockedNotify: vi.fn(),
}))
vi.mock('~/services/hh-service', () => ({
  getHHEventByRegion: mockedGetHHEventByRegion,
  getHHEventRemaining: mockedGetHHEventRemaining,
}))
vi.mock('~/services/notification-service', () => ({
  notify: mockedNotify,
}))

const userStore = useUserStore(createTestingPinia())
userStore.$patch({ user: { region: Region.Eu } })

it('basic', () => {
  const { HHEvent, HHEventRemaining, isHHCountdownEnded } = useHappyHours()
  expect(HHEvent.value).toEqual({
    end: new Date('2000-02-02T00:00:00.000Z'),
    start: new Date('2000-02-01T00:00:00.000Z'),
  })
  expect(HHEventRemaining.value).toEqual(1000)
  expect(isHHCountdownEnded.value).toEqual(false)
})

it('onStartHHCountdown, onEndHHCountdown', () => {
  const { isHHCountdownEnded, onEndHHCountdown, onStartHHCountdown } = useHappyHours()

  onStartHHCountdown()
  onStartHHCountdown()

  expect(mockedNotify).toHaveBeenCalledTimes(1)
  expect(mockedNotify).toHaveBeenNthCalledWith(1, 'hh.notify.started')
  expect(isHHCountdownEnded.value).toEqual(false)

  onEndHHCountdown()

  expect(mockedNotify).toHaveBeenNthCalledWith(2, 'hh.notify.ended')
  expect(isHHCountdownEnded.value).toEqual(true)
})

it('transformSlotProps', () => {
  const { transformSlotProps } = useHappyHours()

  expect(
    transformSlotProps({
      h: 8,
      m: 12,
    }),
  ).toEqual({
    h: '08',
    m: '12',
  })
})
