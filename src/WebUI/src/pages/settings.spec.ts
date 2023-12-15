import { createTestingPinia } from '@pinia/testing';
import { Region } from '@/models/region';
import { User } from '@/models/user';
import { mountWithRouter } from '@/__test__/unit/utils';

const { mockedNotify, mockedDeleteUser, mockedLogout, mockedHide } = vi.hoisted(() => ({
  mockedNotify: vi.fn(),
  mockedDeleteUser: vi.fn(),
  mockedLogout: vi.fn(),
  mockedHide: vi.fn(),
}));
vi.mock('@/services/notification-service', () => ({
  notify: mockedNotify,
}));
vi.mock('@/services/users-service', () => ({
  deleteUser: mockedDeleteUser,
}));
vi.mock('@/services/auth-service', () => ({
  logout: mockedLogout,
}));

import { useUserStore } from '@/stores/user';

const userStore = useUserStore(createTestingPinia());
userStore.$patch({ user: { region: Region.Eu } as User });

import Page from './settings.vue';
import { Character } from '@/models/character';

const routes = [
  {
    name: 'settings',
    path: '/settings',
    component: Page,
  },
];
const route = {
  name: 'settings',
};
const mountOptions = {
  global: {
    stubs: {
      'i18n-t': {
        template: `<div data-aq-i18n-t-stub>
                    <slot name="link"/>
                  </div>`,
      },
      FormGroup: {
        template: `<div data-aq-FormGroup-stub>
          <slot />
        </div>`,
      },
      Modal: {
        setup() {
          return {
            hide: mockedHide,
          };
        },
        template: `<div data-aq-modal-stub>
                    <slot v-bind="{ hide }" />
                    <slot name="popper" v-bind="{ hide }" />
                  </div>`,
      },
    },
  },
};

it('can delete user', async () => {
  const USER_NAME = 'Fluttershy';
  userStore.user = { name: USER_NAME } as User;

  const { wrapper } = await mountWithRouter(mountOptions, routes, route);

  expect(wrapper.find('[data-aq-cant-delete-user-message]').exists()).toBeFalsy();
  expect(wrapper.find('[data-aq-delete-user-group]').classes()).not.toContain(
    'pointer-events-none'
  );
  expect(wrapper.find('[data-aq-modal-stub]').attributes().disabled).toEqual('false');

  const ConfirmActionForm = wrapper.findComponent({ name: 'ConfirmActionForm' });

  expect(ConfirmActionForm.props('name')).toEqual(
    `user.settings.delete.dialog.enterToConfirm::user:${USER_NAME}`
  );

  await ConfirmActionForm.vm.$emit('confirm');

  expect(mockedDeleteUser).toBeCalled();
  expect(mockedNotify).toBeCalledWith('user.settings.delete.notify.success');
  expect(mockedLogout).toBeCalled();
  expect(mockedHide).toBeCalled();
});

it('can`t delete user', async () => {
  const USER_NAME = 'Fluttershy';
  userStore.user = { name: USER_NAME } as User;
  userStore.characters = [{ id: 1 }] as Character[];

  const { wrapper } = await mountWithRouter(mountOptions, routes, route);

  expect(wrapper.find('[data-aq-cant-delete-user-message]').exists()).toBeTruthy();
  expect(wrapper.find('[data-aq-delete-user-group]').classes()).toContain('pointer-events-none');
  expect(wrapper.find('[data-aq-modal-stub]').attributes().disabled).toEqual('true');
});
