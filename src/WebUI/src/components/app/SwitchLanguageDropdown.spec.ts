import { mount } from '@vue/test-utils'

import SwitchLanguageDropdown from './SwitchLanguageDropdown.vue'

const { mockedCurrentLocale, mockedSupportedLocales, mockedSwitchLanguage } = vi.hoisted(() => ({
  mockedCurrentLocale: vi.fn().mockReturnValue('unicorn'),
  mockedSupportedLocales: vi.fn().mockReturnValue(['unicorn', 'pony']),
  mockedSwitchLanguage: vi.fn(),
}))
vi.mock('~/services/translate-service', () => ({
  currentLocale: mockedCurrentLocale,
  supportedLocales: mockedSupportedLocales,
  switchLanguage: mockedSwitchLanguage,
}))

it('switch lang', async () => {
  const wrapper = mount(SwitchLanguageDropdown)

  const langItems = wrapper.findAll('[data-aq-switch-lang-item]')

  // checked icon
  expect(langItems.at(0)!.attributes('checked')).toEqual('true')
  expect(langItems.at(1)!.attributes('checked')).toEqual('false')

  await langItems.at(1)!.trigger('click')

  expect(mockedSwitchLanguage).toBeCalledWith('pony')
})
