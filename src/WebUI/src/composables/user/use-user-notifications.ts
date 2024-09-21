import { useAsyncState } from '@vueuse/core';
import {
  getUserNotifications,
  readUserNotification,
  readAllUserNotifications,
  deleteUserNotification,
  deleteAllUserNotifications,
} from '@/services/users-service';
import { NotificationState } from '@/models/notificatios';
import { useAsyncCallback } from '@/utils/useAsyncCallback';
import { useUserStore } from '@/stores/user';

export const useUsersNotifications = () => {
  const userStore = useUserStore();

  const {
    state: notifications,
    execute: loadNotifications,
    isLoading: loadingNotifications,
  } = useAsyncState(
    () => getUserNotifications(),
    {
      notifications: [],
      dict: {
        users: [],
        clans: [],
        characters: [],
      },
    },
    {
      resetOnExecute: false,
    }
  );

  const hasUnreadNotifications = computed(() =>
    notifications.value.notifications.some(n => n.state === NotificationState.Unread)
  );

  const { execute: readNotification, loading: readingNotification } = useAsyncCallback(
    async (id: number) => {
      await readUserNotification(id);

      await Promise.all([loadNotifications(), userStore.fetchUser()]);
    }
  );

  const { execute: readAllNotifications, loading: readingAllNotification } = useAsyncCallback(
    async () => {
      await readAllUserNotifications();
      await Promise.all([loadNotifications(), userStore.fetchUser()]);
    }
  );

  const { execute: deleteNotification, loading: deletingNotification } = useAsyncCallback(
    async (id: number) => {
      await deleteUserNotification(id);
      await Promise.all([loadNotifications(), userStore.fetchUser()]);
    }
  );

  const { execute: deleteAllNotifications, loading: deletingAllNotification } = useAsyncCallback(
    async () => {
      await deleteAllUserNotifications();
      await Promise.all([loadNotifications(), userStore.fetchUser()]);
    }
  );

  const isLoading = computed(
    () =>
      loadingNotifications.value ||
      readingNotification.value ||
      deletingNotification.value ||
      readingAllNotification.value ||
      deletingAllNotification.value
  );

  const isEmpty = computed(() => !Boolean(notifications.value.notifications.length));

  return {
    notifications,
    isEmpty,
    isLoading,
    loadNotifications,
    hasUnreadNotifications,
    readNotification,
    readAllNotifications,
    deleteNotification,
    deleteAllNotifications,
  };
};
