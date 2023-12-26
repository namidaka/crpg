export const i18nTMock = vi.fn((val, params = null) =>
  [val, params ? Object.entries(params).map(([key, val]) => `${key}:${val}`) : null]
    .filter(Boolean)
    .join('::')
);
export const i18nNMock = vi.fn(val => String(val));
export const i18nDMock = vi.fn(val => val);
