import { mount } from '@vue/test-utils';

const { mockedSupportedLocales, mockedSwitchLanguage, mockedCurrentLocale } = vi.hoisted(() => ({
  mockedSupportedLocales: vi.fn().mockReturnValue(['unicorn', 'pony']),
  mockedSwitchLanguage: vi.fn(),
  mockedCurrentLocale: vi.fn().mockReturnValue('unicorn'),
}));
vi.mock('@/services/translate-service', () => ({
  supportedLocales: mockedSupportedLocales,
  currentLocale: mockedCurrentLocale,
  switchLanguage: mockedSwitchLanguage,
}));

import SwitchLanguageDropdown from './SwitchLanguageDropdown.vue';

it('switch lang', async () => {
  const wrapper = mount(SwitchLanguageDropdown);

  const langItems = wrapper.findAll('[data-aq-switch-lang-item]');

  // checked icon
  expect(langItems.at(0)!.attributes('checked')).toEqual('true');
  expect(langItems.at(1)!.attributes('checked')).toEqual('false');

  await langItems.at(1)!.trigger('click');

  expect(mockedSwitchLanguage).toBeCalledWith('pony');
});
