import { createTestingPinia } from '@pinia/testing';
import { useUserStore } from '@/stores/user';

const { mockedGetHHEventByRegion, mockedGetHHEventRemaining, mockedNotify } = vi.hoisted(() => ({
  mockedGetHHEventByRegion: vi.fn().mockReturnValue({
    start: new Date('2000-02-01T00:00:00.000Z'),
    end: new Date('2000-02-02T00:00:00.000Z'),
  }),
  mockedGetHHEventRemaining: vi.fn().mockReturnValue(1000),
  mockedNotify: vi.fn(),
}));
vi.mock('@/services/hh-service', () => ({
  getHHEventByRegion: mockedGetHHEventByRegion,
  getHHEventRemaining: mockedGetHHEventRemaining,
}));
vi.mock('@/services/notification-service', () => ({
  notify: mockedNotify,
}));

const userStore = useUserStore(createTestingPinia());
userStore.$patch({ user: { region: Region.Eu } });

import { useHappyHours } from './use-hh';
import { Region } from '@/models/region';

it('basic', () => {
  const { HHEvent, HHEventRemaining, isHHCountdownEnded } = useHappyHours();
  expect(HHEvent.value).toEqual({
    start: new Date('2000-02-01T00:00:00.000Z'),
    end: new Date('2000-02-02T00:00:00.000Z'),
  });
  expect(HHEventRemaining.value).toEqual(1000);
  expect(isHHCountdownEnded.value).toEqual(false);
});

it('onStartHHCountdown, onEndHHCountdown', () => {
  const { onStartHHCountdown, onEndHHCountdown, isHHCountdownEnded } = useHappyHours();

  onStartHHCountdown();
  onStartHHCountdown();

  expect(mockedNotify).toHaveBeenCalledTimes(1);
  expect(mockedNotify).toHaveBeenNthCalledWith(1, 'hh.notify.started');
  expect(isHHCountdownEnded.value).toEqual(false);

  onEndHHCountdown();

  expect(mockedNotify).toHaveBeenNthCalledWith(2, 'hh.notify.ended');
  expect(isHHCountdownEnded.value).toEqual(true);
});

it('transformSlotProps', () => {
  const { transformSlotProps } = useHappyHours();

  expect(
    transformSlotProps({
      h: 8,
      m:12
    })
  ).toEqual({
    h: '08',
    m: '12',
  });
});
