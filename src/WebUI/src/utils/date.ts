import { DateTime } from 'luxon';
import { HumanDuration } from '@/models/datetime';

export const parseTimestamp = (ts: number): HumanDuration => {
  const days = Math.floor(ts / 86400000);
  const hours = Math.floor((ts % 86400000) / 3600000);
  const minutes = Math.floor(((ts % 86400000) % 3600000) / 60000);

  return {
    days,
    hours,
    minutes,
  };
};

const daysToMs = (days: number) => days * 24 * 60 * 60 * 1000;

const hoursToMs = (hours: number) => hours * 60 * 60 * 1000;

const minutesToMs = (minutes: number) => minutes * 60 * 1000;

export const msToHours = (ms: number) => Math.floor(ms / 60 / 60 / 1000);

export const convertHumanDurationToMs = (duration: HumanDuration) => {
  return daysToMs(duration.days) + hoursToMs(duration.hours) + minutesToMs(duration.minutes);
};

/**
 * @param {number} duration - ms
 */
export const checkIsDateExpired = (createdAt: Date, duration: number) => {
  return new Date().getTime() > new Date(createdAt).getTime() + duration;
};

/**
 * @param {number} duration - ms
 */
export const computeLeftMs = (createdAt: Date, duration: number) => {
  const result = new Date(createdAt).getTime() + duration - new Date().getTime();
  return result < 0 ? 0 : result;
};

export const isBetween = (date: Date, start: Date, end: Date) =>
  date.valueOf() >= start.valueOf() && date.valueOf() <= end.valueOf();

// https://medium.com/@vladkens/automatic-parsing-of-date-strings-in-rest-protocol-with-typescript-cf43554bd157
const ISODateFormat = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d*)?(?:[-+]\d{2}:?\d{2}|Z)?$/;
const isIsoDateString = (value: unknown): value is string => {
  return typeof value === 'string' && ISODateFormat.test(value);
};
export const JSONDateToJs = (data: unknown) => {
  if (isIsoDateString(data)) return DateTime.fromISO(data).toJSDate();
  if (data === null || data === undefined || typeof data !== 'object') return data;
  for (const [key, val] of Object.entries(data)) {
    // @ts-expect-error this is a hack to make the type checker happy
    if (isIsoDateString(val)) data[key] = DateTime.fromISO(val).toJSDate();
    else if (typeof val === 'object') JSONDateToJs(val);
  }
  return data;
};
