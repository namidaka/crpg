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

export const useUsersNotifications = () => {
  const {
    state: notifications,
    execute: loadNotifications,
    isLoading: loadingNotifications,
  } = useAsyncState(
    () => getUserNotifications(),
    {
      notifications: [],
      users: [],
      clans: [],
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
      await loadNotifications();
    }
  );
  const { execute: readAllNotifications, loading: readingAllNotification } = useAsyncCallback(
    async () => {
      await readAllUserNotifications();
      await loadNotifications();
    }
  );

  const { execute: deleteNotification, loading: deletingNotification } = useAsyncCallback(
    async (id: number) => {
      await deleteUserNotification(id);
      await loadNotifications();
    }
  );

  const { execute: deleteAllNotifications, loading: deletingAllNotification } = useAsyncCallback(
    async () => {
      await deleteAllUserNotifications();
      await loadNotifications();
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
