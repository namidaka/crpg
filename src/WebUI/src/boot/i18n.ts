import { createI18n } from 'vue-i18n'

import type { BootModule } from '~/types/boot-module'

export const i18n = createI18n({
  fallbackLocale: import.meta.env.VITE_LOCALE_FALLBACK,
  globalInjection: true,
  locale: import.meta.env.VITE_LOCALE_DEFAULT,
  missingWarn: false,
  fallbackWarn: false,
  datetimeFormats: {
    cn: {
      long: {
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric',
        month: 'short',
        weekday: 'short',
        year: 'numeric',
      },
      short: {
        dateStyle: 'short',
      },
      time: {
        hour: 'numeric',
        minute: 'numeric',
      },
    },
    en: {
      long: {
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric',
        month: 'short',
        weekday: 'short',
        year: 'numeric',
      },
      short: {
        day: 'numeric',
        hour: 'numeric',
        hour12: false,
        minute: 'numeric',
        month: 'numeric',
        year: 'numeric',
      },
      time: {
        hour: 'numeric',
        minute: 'numeric',
      },
    },
    ru: {
      long: {
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric',
        month: 'short',
        weekday: 'short',
        year: 'numeric',
      },
      short: {
        dateStyle: 'short',
      },
      time: {
        hour: 'numeric',
        minute: 'numeric',
      },
    },
  },
  messages: {},
  numberFormats: {
    cn: {
      decimal: {
        maximumFractionDigits: 3,
        style: 'decimal',
      },
      percent: {
        minimumFractionDigits: 2,
        style: 'percent',
      },
      second: {
        maximumFractionDigits: 3,
        style: 'unit',
        unit: 'second',
        unitDisplay: 'narrow',
      },
    },
    en: {
      decimal: {
        maximumFractionDigits: 3,
        style: 'decimal',
      },
      percent: {
        minimumFractionDigits: 2,
        style: 'percent',
      },
      second: {
        maximumFractionDigits: 3,
        style: 'unit',
        unit: 'second',
        unitDisplay: 'narrow',
      },
    },
    ru: {
      decimal: {
        maximumFractionDigits: 3,
        style: 'decimal',
      },
      percent: {
        minimumFractionDigits: 2,
        style: 'percent',
      },
      second: {
        maximumFractionDigits: 3,
        style: 'unit',
        unit: 'second',
        unitDisplay: 'narrow',
      },
    },
  },
  pluralRules: {
    ru: (choice: number, choicesLength: number) => {
      if (choice === 0) {
        return 0
      }

      const teen = choice > 10 && choice < 20
      const endsWithOne = choice % 10 === 1
      if (!teen && endsWithOne) {
        return 1
      }
      if (!teen && choice % 10 >= 2 && choice % 10 <= 4) {
        return 2
      }

      return choicesLength < 4 ? 2 : 3
    },
  },
})

function loadLocaleMessages(locale: string) {
  return import(`../../locales/${locale}.yml`)
}

export const install: BootModule = async (app) => {
  if (i18n.global.fallbackLocale.value && i18n.global.fallbackLocale.value !== i18n.global.locale.value) {
    const fallbackMessages = await loadLocaleMessages(i18n.global.fallbackLocale.value)
    i18n.global.setLocaleMessage(i18n.global.fallbackLocale.value, fallbackMessages.default)
  }
  const messages = await loadLocaleMessages(i18n.global.locale.value)
  i18n.global.setLocaleMessage(i18n.global.locale.value, messages.default)

  app.use(i18n)
}
