import type { NavigationGuard, RouteRecordRaw } from 'vue-router/auto'

import { setupLayouts } from 'virtual:generated-layouts'
import { createRouter, createWebHistory } from 'vue-router/auto'
import { routes } from 'vue-router/auto-routes'

import type { BootModule } from '~/types/boot-module'

import { authRouterMiddleware, signInCallback } from '~/middlewares/auth'
import { activeCharacterRedirect, characterValidate } from '~/middlewares/character'
import {
  canManageApplications,
  canUpdateClan,
  canUseClanArmory,
  clanExistValidate,
  clanIdParamValidate,
} from '~/middlewares/clan'
import { parseQuery, scrollBehavior, stringifyQuery } from '~/utils/router'

const getRouteMiddleware = (name: string) => {
  const middlewareMap: Record<string, NavigationGuard> = {
    activeCharacterRedirect,

    canManageApplications,
    canUpdateClan,

    canUseClanArmory,
    characterValidate,
    clanExistValidate,
    clanIdParamValidate,
    signInCallback,
  }

  return middlewareMap[name]
}

// TODO: FIXME: SPEC
const setRouteMiddleware = (routes: RouteRecordRaw[]) => {
  routes.forEach((route) => {
    if (route.children !== undefined) {
      setRouteMiddleware(route.children)
    }

    if (route.meta?.middleware === undefined) { return }
    route.beforeEnter = getRouteMiddleware(route.meta.middleware as string)
  })
}

export const install: BootModule = (app) => {
  setRouteMiddleware(routes)

  const router = createRouter({
    history: createWebHistory(),
    scrollBehavior,
    /* A custom parse/stringify query is needed because by default
    ?types=HeadArmor&types=ShoulderArmor is parsed correctly as ["HeadArmor", "ShoulderArmor"]
    but ?types=HeadArmor is parsed as "HeadArmor" (not an array).
    To solve this issue qs library adds brackets for arrays ?types[]=HeadArmor.
    ref: https://router.vuejs.org/api/interfaces/RouterOptions.html#parsequery
    spec: src/WebUI/src/utils/router.spec.ts */
    parseQuery,
    routes: setupLayouts(routes),
    stringifyQuery,
  })

  router.beforeEach(authRouterMiddleware)

  app.use(router)
}

// TODO: typed meta
// declare module 'vue-router' {
//   interface RouteMeta {
//   }
// }
