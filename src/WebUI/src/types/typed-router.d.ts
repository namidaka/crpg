// Generated by unplugin-vue-router. ‼️ DO NOT MODIFY THIS FILE ‼️
// It's recommended to commit this file.
// Make sure to add this file to your tsconfig.json file as an "includes" or "files" entry.

/// <reference types="unplugin-vue-router/client" />

import type {
  // type safe route locations
  RouteLocationTypedList,
  RouteLocationResolvedTypedList,
  RouteLocationNormalizedTypedList,
  RouteLocationNormalizedLoadedTypedList,
  RouteLocationAsString,
  RouteLocationAsRelativeTypedList,
  RouteLocationAsPathTypedList,

  // helper types
  // route definitions
  RouteRecordInfo,
  ParamValue,
  ParamValueOneOrMore,
  ParamValueZeroOrMore,
  ParamValueZeroOrOne,

  // vue-router extensions
  _RouterTyped,
  RouterLinkTyped,
  RouterLinkPropsTyped,
  NavigationGuard,
  UseLinkFnTyped,

  // data fetching
  _DataLoader,
  _DefineLoaderOptions,
} from 'unplugin-vue-router/types'

declare module 'vue-router/auto/routes' {
  export interface RouteNamedMap {
    'Root': RouteRecordInfo<'Root', '/', Record<never, never>, Record<never, never>>,
    'PageNotFound': RouteRecordInfo<'PageNotFound', '/:404(.*)?', { 404?: ParamValueZeroOrOne<true> }, { 404?: ParamValueZeroOrOne<false> }>,
    'Banned': RouteRecordInfo<'Banned', '/banned', Record<never, never>, Record<never, never>>,
    'Builder': RouteRecordInfo<'Builder', '/builder', Record<never, never>, Record<never, never>>,
    'CharactersParent': RouteRecordInfo<'CharactersParent', '/characters', Record<never, never>, Record<never, never>>,
    'Characters': RouteRecordInfo<'Characters', '/characters', Record<never, never>, Record<never, never>>,
    'CharactersIdParent': RouteRecordInfo<'CharactersIdParent', '/characters/:id', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    'CharactersId': RouteRecordInfo<'CharactersId', '/characters/:id', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    'CharactersIdCharacteristic': RouteRecordInfo<'CharactersIdCharacteristic', '/characters/:id/characteristic', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    'CharactersIdInventory': RouteRecordInfo<'CharactersIdInventory', '/characters/:id/inventory', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    'Clans': RouteRecordInfo<'Clans', '/clans', Record<never, never>, Record<never, never>>,
    'ClansId': RouteRecordInfo<'ClansId', '/clans/:id', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    'ClansIdApplications': RouteRecordInfo<'ClansIdApplications', '/clans/:id/applications', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    'ClansIdUpdate': RouteRecordInfo<'ClansIdUpdate', '/clans/:id/update', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    'ClansCreate': RouteRecordInfo<'ClansCreate', '/clans/create', Record<never, never>, Record<never, never>>,
    'ModeratorParent': RouteRecordInfo<'ModeratorParent', '/moderator', Record<never, never>, Record<never, never>>,
    'Moderator': RouteRecordInfo<'Moderator', '/moderator', Record<never, never>, Record<never, never>>,
    'ModeratorFindUser': RouteRecordInfo<'ModeratorFindUser', '/moderator/find-user', Record<never, never>, Record<never, never>>,
    'ModeratorUserId': RouteRecordInfo<'ModeratorUserId', '/moderator/user/:id', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    'ModeratorUserIdActivityLogs': RouteRecordInfo<'ModeratorUserIdActivityLogs', '/moderator/user/:id/activity-logs', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    'ModeratorUserIdRestrictions': RouteRecordInfo<'ModeratorUserIdRestrictions', '/moderator/user/:id/restrictions', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    'PrivacyPolicy': RouteRecordInfo<'PrivacyPolicy', '/privacy-policy', Record<never, never>, Record<never, never>>,
    'Settings': RouteRecordInfo<'Settings', '/settings', Record<never, never>, Record<never, never>>,
    'Shop': RouteRecordInfo<'Shop', '/shop', Record<never, never>, Record<never, never>>,
    'SigninCallback': RouteRecordInfo<'SigninCallback', '/signin-callback', Record<never, never>, Record<never, never>>,
    'SigninSilentCallback': RouteRecordInfo<'SigninSilentCallback', '/signin-silent-callback', Record<never, never>, Record<never, never>>,
    'TermsOfService': RouteRecordInfo<'TermsOfService', '/terms-of-service', Record<never, never>, Record<never, never>>,
  }
}

declare module 'vue-router/auto' {
  import type { RouteNamedMap } from 'vue-router/auto/routes'

  export type RouterTyped = _RouterTyped<RouteNamedMap>

  /**
   * Type safe version of `RouteLocationNormalized` (the type of `to` and `from` in navigation guards).
   * Allows passing the name of the route to be passed as a generic.
   */
  export type RouteLocationNormalized<Name extends keyof RouteNamedMap = keyof RouteNamedMap> = RouteLocationNormalizedTypedList<RouteNamedMap>[Name]

  /**
   * Type safe version of `RouteLocationNormalizedLoaded` (the return type of `useRoute()`).
   * Allows passing the name of the route to be passed as a generic.
   */
  export type RouteLocationNormalizedLoaded<Name extends keyof RouteNamedMap = keyof RouteNamedMap> = RouteLocationNormalizedLoadedTypedList<RouteNamedMap>[Name]

  /**
   * Type safe version of `RouteLocationResolved` (the returned route of `router.resolve()`).
   * Allows passing the name of the route to be passed as a generic.
   */
  export type RouteLocationResolved<Name extends keyof RouteNamedMap = keyof RouteNamedMap> = RouteLocationResolvedTypedList<RouteNamedMap>[Name]

  /**
   * Type safe version of `RouteLocation` . Allows passing the name of the route to be passed as a generic.
   */
  export type RouteLocation<Name extends keyof RouteNamedMap = keyof RouteNamedMap> = RouteLocationTypedList<RouteNamedMap>[Name]

  /**
   * Type safe version of `RouteLocationRaw` . Allows passing the name of the route to be passed as a generic.
   */
  export type RouteLocationRaw<Name extends keyof RouteNamedMap = keyof RouteNamedMap> =
    | RouteLocationAsString<RouteNamedMap>
    | RouteLocationAsRelativeTypedList<RouteNamedMap>[Name]
    | RouteLocationAsPathTypedList<RouteNamedMap>[Name]

  /**
   * Generate a type safe params for a route location. Requires the name of the route to be passed as a generic.
   */
  export type RouteParams<Name extends keyof RouteNamedMap> = RouteNamedMap[Name]['params']
  /**
   * Generate a type safe raw params for a route location. Requires the name of the route to be passed as a generic.
   */
  export type RouteParamsRaw<Name extends keyof RouteNamedMap> = RouteNamedMap[Name]['paramsRaw']

  export function useRouter(): RouterTyped
  export function useRoute<Name extends keyof RouteNamedMap = keyof RouteNamedMap>(name?: Name): RouteLocationNormalizedLoadedTypedList<RouteNamedMap>[Name]

  export const useLink: UseLinkFnTyped<RouteNamedMap>

  export function onBeforeRouteLeave(guard: NavigationGuard<RouteNamedMap>): void
  export function onBeforeRouteUpdate(guard: NavigationGuard<RouteNamedMap>): void

  export const RouterLink: RouterLinkTyped<RouteNamedMap>
  export const RouterLinkProps: RouterLinkPropsTyped<RouteNamedMap>

  // Experimental Data Fetching

  export function defineLoader<
    P extends Promise<any>,
    Name extends keyof RouteNamedMap = keyof RouteNamedMap,
    isLazy extends boolean = false,
  >(
    name: Name,
    loader: (route: RouteLocationNormalizedLoaded<Name>) => P,
    options?: _DefineLoaderOptions<isLazy>,
  ): _DataLoader<Awaited<P>, isLazy>
  export function defineLoader<
    P extends Promise<any>,
    isLazy extends boolean = false,
  >(
    loader: (route: RouteLocationNormalizedLoaded) => P,
    options?: _DefineLoaderOptions<isLazy>,
  ): _DataLoader<Awaited<P>, isLazy>

  export {
    _definePage as definePage,
    _HasDataLoaderMeta as HasDataLoaderMeta,
    _setupDataFetchingGuard as setupDataFetchingGuard,
    _stopDataFetchingScope as stopDataFetchingScope,
  } from 'unplugin-vue-router/runtime'
}

declare module 'vue-router' {
  import type { RouteNamedMap } from 'vue-router/auto/routes'

  export interface TypesConfig {
    beforeRouteUpdate: NavigationGuard<RouteNamedMap>
    beforeRouteLeave: NavigationGuard<RouteNamedMap>

    $route: RouteLocationNormalizedLoadedTypedList<RouteNamedMap>[keyof RouteNamedMap]
    $router: _RouterTyped<RouteNamedMap>

    RouterLink: RouterLinkTyped<RouteNamedMap>
  }
}
