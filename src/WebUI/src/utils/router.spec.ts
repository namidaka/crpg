import type { LocationQuery, RouteLocationNormalizedLoaded } from 'vue-router/auto'

import { parseQuery, scrollBehavior, stringifyQuery } from './router'

it.each<[string, any]>([
  [
    'search=Lion%20Imprinted%20Saber',
    {
      search: 'Lion Imprinted Saber',
    },
  ],
  [
    'type=Bow&filter[flags][]=bow&filter[flags][]=long_bow',
    {
      filter: {
        flags: ['bow', 'long_bow'],
      },
      type: 'Bow',
    },
  ],
  [
    'type=Bolts&filter[damageType][]=Cut&filter[stackWeight][]=0.2&filter[stackWeight][]=2&search=Bolts&hideOwnedItems=true',
    {
      filter: {
        damageType: ['Cut'],
        stackWeight: [0.2, 2], // custom decoder
      },
      hideOwnedItems: true, // custom decoder
      search: 'Bolts',
      type: 'Bolts',
    },
  ],
])('parseQuery - q: %s', (query, expectation) => {
  expect(parseQuery(query)).toEqual(expectation)
})

it.todo.each<[LocationQuery, string]>([
  [
    {
      search: 'Lion Imprinted Saber',
    },
    'search=Lion%20Imprinted%20Saber',
  ],
])('stringifyQuery - q: %s', (query, expectation) => {
  expect(stringifyQuery(query)).toEqual(expectation)
})

describe('scrollBehavior', () => {
  it('savedPosition', () => {
    expect(
      scrollBehavior({} as RouteLocationNormalizedLoaded, {} as RouteLocationNormalizedLoaded, {
        left: 0,
        top: 20,
      }),
    ).toEqual({ left: 0, top: 20 })
  })

  it('scrollToTop', () => {
    expect(
      scrollBehavior(
        {
          matched: [
            {
              meta: {
                scrollToTop: true,
              },
            },
          ],
        } as unknown as RouteLocationNormalizedLoaded,
        {} as RouteLocationNormalizedLoaded,
        null,
      ),
    ).toEqual({ behavior: 'smooth', left: 0, top: 0 })
  })
})
