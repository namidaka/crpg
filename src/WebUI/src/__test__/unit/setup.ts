// Mock fetch API
// https://github.com/sheremet-va/vi-fetch
import 'vi-fetch/setup'
import Oruga from '@oruga-ui/oruga-next'
import { config } from '@vue/test-utils'
import { mockFetch } from 'vi-fetch'
import { createI18n } from 'vue-i18n'

import mockConstants from '~/__mocks__/constants.json'
import { i18nDMock, i18nNMock, i18nTMock } from '~/__mocks__/i18n'

const mockMatchMedia = vi.fn().mockImplementation(query => ({
  addEventListener: vi.fn(),
  addListener: vi.fn(), // deprecated
  dispatchEvent: vi.fn(),
  matches: false,
  media: query,
  onchange: null,
  removeEventListener: vi.fn(),
  removeListener: vi.fn(), // deprecated
}))
vi.stubGlobal('matchMedia', mockMatchMedia)

vi.mock(
  '~/services/translate-service',
  vi.fn().mockImplementation(() => ({
    d: i18nDMock,
    n: i18nNMock,
    t: i18nTMock,
  })),
)

vi.mock(
  '~root/data/constants.json',
  vi.fn().mockImplementation(() => mockConstants),
)

mockFetch.setOptions({
  baseUrl: import.meta.env.VITE_API_BASE_URL,
})

const FakeInput = {
  props: ['modelValue', 'size'],
  template: `<input :value="modelValue" @input="$emit('update:modelValue', $event.target.value)" />`,
}

// TODO:
const FakeCheckBox = {
  props: ['modelValue'],
  template: `<input :value="Boolean(modelValue)" @input="$emit('update:modelValue', Boolean($event.target.value))" />`,
}

const FakeRadioBox = {
  props: ['modelValue', 'nativeValue'],
  template: `<input type="radio" :value="nativeValue" @change="$emit('update:modelValue', $event.target.value)" />`,
}

const FakeBtn = {
  props: ['nativeType'],
  template: `<button :type="nativeType" />`,
}

config.global.plugins = [
  Oruga,
  createI18n({ fallbackWarn: false, legacy: false, missingWarn: false }),
]

config.global.mocks = {
  $d: i18nDMock,
  $n: i18nNMock,
  $t: i18nTMock,
}

config.global.stubs = {
  'OButton': FakeBtn,
  'OCheckbox': FakeCheckBox,
  'ODateTimePicker': true,
  'OField': {
    template: `<div><div data-aq-o-field-stub-message-slot><slot name="message"/></div><slot /></div>`,
  },
  'OIcon': true,
  'OInput': FakeInput,
  'OPagination': true,
  'ORadio': FakeRadioBox,
  'OSwitch': FakeCheckBox,

  'RouterLink': true,
  //
  'OTabItem': false,
  'OTable': false,
  'OTableColumn': false,
  'OTabs': false,

  //
  'DropdownItem': true,
  'FontAwesomeIcon': true,

  'FontAwesomeLayers': true,
  'i18n-t': {
    template: `<div data-aq-i18n-t-stub><slot /><slot name="link"/></div>`,
  },
  'Modal': {
    template: `<div data-aq-modal-stub><slot /><slot name="popper" v-bind="{ hide: () => {} }" /></div>`,
  },
  'VDropdown': {
    template: `<div data-aq-vdropdown-stub><slot /><slot name="popper" v-bind="{ hide: () => {} }" /></div>`,
  },
}

config.global.directives = {
  tooltip: {
    beforeMount(el: Element) {
      el.setAttribute('data-aq-with-tooltip', 'true')
    },
  },
}

vi.mock('@vueuse/head', () => ({
  useHead: vi.fn(),
}))

beforeEach(() => {
  mockFetch.clearAll()
})
