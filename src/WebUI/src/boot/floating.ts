import FloatingVue from 'floating-vue'

import type { BootModule } from '~/types/boot-module'

export const install: BootModule = (app) => {
  app.use(FloatingVue, {
    disposeTimeout: 100,
    distance: 16,
  })
}
