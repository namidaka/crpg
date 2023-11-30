import { useStorage } from '@vueuse/core';
import { useUserStore } from '@/stores/user';

// TODO: notification center https://github.com/namidaka/crpg/issues/186
export const useWelcome = () => {
  const showedWelcomeMessage = useStorage<boolean>('user-welcome-message-showed', false);

  const userStore = useUserStore();

  const shownWelcomeMessage = computed(
    () => !showedWelcomeMessage.value && userStore.user !== null && userStore.isRecentUser
  );

  const onCloseWelcomeMessage = () => {
    showedWelcomeMessage.value = true;
  };

  const showWelcomeMessage = () => {
    showedWelcomeMessage.value = false;
  };

  return {
    shownWelcomeMessage,
    onCloseWelcomeMessage,
    showWelcomeMessage,
  };
};
