import type { MountingOptions, VueWrapper } from '@vue/test-utils'
import type { ComponentPublicInstance } from 'vue'
import type {
  RouteLocationRaw,
  Router,
  RouteRecordRaw,
} from 'vue-router'

import { flushPromises, mount } from '@vue/test-utils'
import defu from 'defu'
import { defineComponent } from 'vue'
import {
  createRouter,
  createWebHistory,
} from 'vue-router'

// ref: https://github.com/vuejs/test-utils/issues/108#issuecomment-1124851726
export const mountWithRouter = async <Props>(
  options: MountingOptions<Props> = {},
  routes: RouteRecordRaw[],
  route: RouteLocationRaw,
): Promise<{
  wrapper: VueWrapper<ComponentPublicInstance>
  router: Router
}> => {
  const router = createRouter({
    history: createWebHistory(),
    routes,
  })

  router.push(route)

  await router.isReady()

  const app = defineComponent({
    template: `
        <router-view v-slot="{ Component }">
            <suspense>
                <component :is="Component" />
            </suspense>
        </router-view>
    `,
  })

  const wrapper = mount(
    app,
    defu(
      {
        global: {
          plugins: [router],
        },
      },
      options,
    ),
  )

  await flushPromises()

  return { router, wrapper }
}
