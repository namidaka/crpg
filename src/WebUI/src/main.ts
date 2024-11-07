import { createApp } from 'vue'

import { guessDefaultLocale, switchLanguage } from '~/services/translate-service'

import type { BootModule } from './types/boot-module'

import 'floating-vue/dist/style.css'

import './assets/styles/tailwind.css'
import './assets/themes/oruga-tailwind/index.css'

import 'virtual:svg-icons-register'

import App from './App.vue'

const app = createApp(App)

// Load modules || plugins
Object.values(
  import.meta.glob<BootModule>('./boot/*.ts', { eager: true, import: 'install' }),
).forEach(install => install(app))

// eslint-disable-next-line antfu/no-top-level-await
await switchLanguage(guessDefaultLocale())

app.mount('#app')
