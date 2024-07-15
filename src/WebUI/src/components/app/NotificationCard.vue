<script setup lang="ts">
import { NotificationState } from '@/models/notificatios';
import { type UserNotification, type UserPublic } from '@/models/user';
import { useLocaleTimeAgo } from '@/composables/use-locale-time-ago';
import { ActivityLogMetadataDicts } from '@/models/activity-logs';

const { notification, dict } = defineProps<{
  notification: UserNotification;
  dict: ActivityLogMetadataDicts;
}>();

const timeAgo = useLocaleTimeAgo(notification.createdAt);
const isUnread = computed(() => notification.state === NotificationState.Unread);

const emit = defineEmits<{
  read: [];
  delete: [];
}>();
</script>

<template>
  <div class="relative flex items-start gap-4 rounded-lg bg-base-200 px-3 py-3 text-content-200">
    <div
      class="flex h-8 w-8 min-w-8 items-center justify-center gap-1.5 rounded-full bg-content-600"
    >
      <SvgSpriteImg name="logo" viewBox="0 0 162 124" class="w-3/4" />
    </div>

    <div class="flex-1 space-y-3">
      <ActivityLogMetadata
        :keypath="`notification.tpl.${notification.type}`"
        :activityLog="notification.activityLog"
        v-bind="{ dict }"
        class="pr-8"
      />

      <div class="flex items-end gap-4">
        <span
          class="cursor-default text-3xs text-content-300"
          v-tooltip="$d(new Date(notification.createdAt), 'short')"
        >
          {{ timeAgo }}
        </span>

        <div class="ml-auto mr-0 flex gap-3">
          <OButton
            v-if="isUnread"
            variant="transparent"
            size="xs"
            :label="`Read`"
            @click="$emit('read')"
          />
          <OButton
            variant="transparent"
            outlined
            size="xs"
            icon-left="close"
            :label="`Delete`"
            @click="$emit('delete')"
          />
        </div>
      </div>
    </div>

    <div class="absolute right-3 top-3 z-10">
      <span v-tooltip="`Unread notification`">
        <OIcon
          v-if="isUnread"
          class="ml-auto mr-0"
          icon="rare-duotone"
          size="sm"
          :style="{
            '--fa-primary-opacity': 0.15,
            '--fa-primary-color': 'rgba(255, 255, 255, 1)',
            '--fa-secondary-opacity': 1,
            '--fa-secondary-color': 'rgba(83, 188, 150, 1)',
          }"
        />
      </span>
    </div>
  </div>
</template>
