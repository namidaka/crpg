import type { RouteLocationNormalized } from 'vue-router'

export const getRoute = (
  routePart: Partial<RouteLocationNormalized> = {},
): RouteLocationNormalized => ({
  fullPath: '',
  hash: '',
  matched: [],
  meta: {},
  // @ts-expect-error TODO:
  name: '',
  params: {},
  path: '',
  query: {},
  redirectedFrom: undefined,
  ...routePart,
})

export const next = vi.fn()
