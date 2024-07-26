<script setup lang="ts">
import { useUserStore } from '@/stores/user';

definePage({
  meta: {
    layout: 'default',
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const userStore = useUserStore();
</script>

<template>
  <div class="container">
    <div class="mx-auto max-w-2xl py-12">
      <h1 class="mb-14 text-center text-xl text-content-100">
        {{ $t('user.notifications.title') }}
      </h1>

      <div class="flex flex-col flex-wrap gap-4">
        <NotificationCard
          v-if="Boolean(userStore.notifications.length)"
          v-for="notification in userStore.notifications"
          :notification="notification"
        />
      </div>
    </div>
  </div>
</template>
