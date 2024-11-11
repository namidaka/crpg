import type { LocationQuery, RouterScrollBehavior } from 'vue-router/auto'

import { parse, stringify } from 'qs'

const numberCandidate = (candidate: string) => /^[+-]?\d+(?:\.\d+)?$/.test(candidate)

const tryParseFloat = (str: string) => (numberCandidate(str) ? Number.parseFloat(str) : str)

// ref: https://github.com/ljharb/qs/blob/main/lib/utils.js#L111
const decoder = (str: string): string | number | boolean | null | undefined => {
  const candidateToNumber = tryParseFloat(str)

  if (typeof candidateToNumber === 'number' && !Number.isNaN(candidateToNumber)) {
    return candidateToNumber
  }

  const keywords: Record<string, any> = {
    false: false,
    null: null,
    true: true,
    undefined,
  }

  if (str in keywords) {
    return keywords[str]
  }

  const strWithoutPlus = str.replace(/\+/g, ' ')

  try {
    return decodeURIComponent(strWithoutPlus)
  }
  // eslint-disable-next-line unused-imports/no-unused-vars
  catch (_e) {
    return strWithoutPlus
  }
}

export const parseQuery = (query: string) =>
  parse(query, {
    decoder,
    ignoreQueryPrefix: true,
    strictNullHandling: true,
  }) as LocationQuery

export const stringifyQuery = (query: Record<string, any>) =>
  stringify(query, {
    arrayFormat: 'brackets',
    encode: false,
    skipNulls: true,
    strictNullHandling: true,
  })

export const scrollBehavior: RouterScrollBehavior = (to, _from, savedPosition) => {
  if (savedPosition) {
    return savedPosition
  }

  // check if any matched route config has meta that requires scrolling to top
  if (to.matched.some(m => m.meta.scrollToTop)) {
    return { behavior: 'smooth', left: 0, top: 0 }
  }
}
