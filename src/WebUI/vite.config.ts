/// <reference types="vitest" />

import type { Plugin } from 'vite'

import VueI18n from '@intlify/unplugin-vue-i18n/vite'
import Vue from '@vitejs/plugin-vue'
import json5 from 'json5'
import process from 'node:process'
import { fileURLToPath } from 'node:url'
import Visualizer from 'rollup-plugin-visualizer'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import { getPascalCaseRouteName, VueRouterAutoImports } from 'unplugin-vue-router'
import VueRouter from 'unplugin-vue-router/vite'
import { defineConfig } from 'vite'
import viteCompression from 'vite-plugin-compression'
import { createSvgIconsPlugin } from 'vite-plugin-svg-icons'
import Layouts from 'vite-plugin-vue-layouts'

// TODO: to libs
function JSON5(): Plugin {
  const fileRegex = /\.json$/

  return {
    enforce: 'pre', // before vite-json
    name: 'vite-plugin-json5',
    transform(src, id) {
      if (fileRegex.test(id)) {
        let value

        try {
          value = json5.parse(src)
        }
        catch (error) {
          console.error(error)
        }

        return {
          code: value ? JSON.stringify(value) : src,
          map: null,
        }
      }
    },
  }
}

const watchIgnored: string[] = []

if (process.env.NODE_ENV !== 'test') {
  watchIgnored.push('**/*.spec.**')
}

// https://vitejs.dev/config/
export default defineConfig({
  build: {
    target: 'esnext',
  },

  preview: {
    host: '0.0.0.0',
    port: 8080,
  },

  plugins: [
    // https://github.com/posva/unplugin-vue-router
    VueRouter({
      dts: 'src/types/typed-router.d.ts',
      exclude: ['**/*.spec*'],
      extensions: ['.vue'],
      getRouteName: getPascalCaseRouteName,
    }),

    Vue({
      template: {
        transformAssetUrls: {
          includeAbsolute: false,
        },
      },
    }),

    // https://github.com/JohnCampionJr/vite-plugin-vue-layouts
    Layouts(),

    // https://github.com/antfu/unplugin-auto-import
    AutoImport({
      dirs: ['src/utils/inject-strict'],
      dts: 'src/types/vite-auto-imports.d.ts',
      imports: [
        'vue',
        VueRouterAutoImports,
        'vue-i18n',
        'pinia',
        'vitest',
        '@vueuse/head',
        {
          '@vueuse/core': ['useAsyncState'],
        },
      ],
      vueTemplate: true,
    }),

    // TODO: maybe uninstall
    // https://github.com/antfu/unplugin-vue-components
    Components({
      dts: 'src/types/vite-components.d.ts',
    }),

    // https://github.com/btd/rollup-plugin-visualizerhttps://github.com/btd/rollup-plugin-visualizer
    Visualizer({
      brotliSize: true,
      gzipSize: true,
      template: 'sunburst',
    }),

    // https://github.com/intlify/bundle-tools/tree/main/packages/unplugin-vue-i18n
    VueI18n({
      compositionOnly: true,
      include: [fileURLToPath(new URL('./locales/**', import.meta.url))],
      runtimeOnly: true,
      strictMessage: false,
    }),

    JSON5(),

    createSvgIconsPlugin({
      iconDirs: [fileURLToPath(new URL('./src/assets/themes/oruga-tailwind/img', import.meta.url))],
    }),

    viteCompression({
      algorithm: 'gzip',
      filter: /\.(js|css|woff2|html)$/i,
    }),
  ],

  server: {
    host: '127.0.0.1',
    port: 8080,
    watch: {
      ignored: watchIgnored,
      usePolling: true,
    },
  },

  // https://vitest.dev/api/
  resolve: {
    alias: {
      '~': fileURLToPath(new URL('./src', import.meta.url)),
      '~root': fileURLToPath(new URL('../../', import.meta.url)),
    },
  },

  test: {
    clearMocks: true,
    coverage: {
      exclude: ['node_modules/', './src/__test__/unit/index.ts', '**/*.spec.ts'],
      provider: 'v8',
      reporter: ['json', 'text', 'html'],
    },
    environment: 'jsdom',
    globals: true,
    include: ['./src/**/*.spec.ts'],
    setupFiles: ['./src/__test__/unit/setup.ts'],
  },
})
