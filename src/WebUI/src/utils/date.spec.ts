import type { HumanDuration } from '~/models/datetime'

import {
  checkIsDateExpired,
  computeLeftMs,
  convertHumanDurationToMs,
  isBetween,
  parseTimestamp,
} from './date'

const [expiredDate, nowDate, futureDate] = [
  '2022-11-27T20:00:00.0000000Z',
  '2022-11-27T21:00:00.0000000Z',
  '2022-11-27T22:00:00.0000000Z',
]

beforeEach(() => {
  vi.useFakeTimers()
})

afterEach(() => {
  vi.useRealTimers()
})

it.each<[[Date, Date, Date], boolean]>([
  [
    [
      new Date('2000-01-01T10:00:00.000Z'),
      new Date('2000-01-01T09:00:00.000Z'),
      new Date('2000-01-01T11:00:00.000Z'),
    ],
    true,
  ],
  [
    [
      new Date('2000-01-01T09:00:00.000Z'),
      new Date('2000-01-01T09:00:00.000Z'),
      new Date('2000-01-01T09:00:00.000Z'),
    ],
    true,
  ],
  [
    [
      new Date('2000-01-01T09:00:00.000Z'),
      new Date('2000-01-01T10:00:00.000Z'),
      new Date('2000-01-01T09:00:00.000Z'),
    ],
    false,
  ],
])('isBetween - dates: %j', (dates, expectation) => {
  expect(isBetween(...dates)).toEqual(expectation)
})

it('computeLeftMs', () => {
  vi.setSystemTime(nowDate)
  expect(computeLeftMs(new Date(nowDate), 1000)).toEqual(1000)
})

it.each<[Date, number, boolean]>([
  [new Date(expiredDate), 1000, true],
  [new Date(nowDate), 1000, false],
  [new Date(futureDate), 1000, false],
])('checkIsDateExpired - date: %s, duration: %s', (date, duration, expectation) => {
  vi.setSystemTime(nowDate)
  expect(checkIsDateExpired(new Date(date), duration)).toEqual(expectation)
})

it.each<[HumanDuration, number]>([
  [{ days: 0, hours: 0, minutes: 1 }, 60000],
  [{ days: 0, hours: 0, minutes: 0 }, 0],
])('convertHumanDurationToMs - humanDateTime: %j', (humanDateTime, expectation) => {
  expect(convertHumanDurationToMs(humanDateTime)).toEqual(expectation)
})

it('parseTimestamp', () => {
  expect(parseTimestamp(60000)).toEqual({
    days: 0,
    hours: 0,
    minutes: 1,
  })
  expect(parseTimestamp(12000000)).toEqual({
    days: 0,
    hours: 3,
    minutes: 20,
  })
})
