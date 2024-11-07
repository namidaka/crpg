declare module '*.vue' {
  import type { DefineComponent } from 'vue'

  const component: DefineComponent<object, object, any>
  export default component
}

interface ImportMetaEnv {
  readonly VITE_HH: string
  readonly VITE_API_BASE_URL: string
  readonly VITE_LOCALE_DEFAULT: string
  readonly VITE_LOCALE_FALLBACK: string
  readonly VITE_LOCALE_SUPPORTED: string
  // more env variables...
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
