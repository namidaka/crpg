<script setup lang="ts">
import { Clan } from '@/models/clan';
import { NotificationState } from '@/models/notificatios';
import { UserNotification, UserPublic } from '@/models/user';
import { useLocaleTimeAgo } from '@/composables/use-locale-time-ago';

const { notification, users, clans } = defineProps<{
  notification: UserNotification;
  users: UserPublic[];
  clans: Clan[];
}>();

const timeAgo = useLocaleTimeAgo(notification.createdAt);

const getClanById = (clanId: number) => clans.find(({ id }) => id === clanId);

const getUserById = (userId: number) => users.find(({ id }) => id === userId);

const isUnread = computed(() => notification.state === NotificationState.Unread);

const emit = defineEmits<{
  read: [];
  delete: [];
}>();
</script>

<template>
  <div class="relative flex gap-4 rounded-lg bg-base-200 px-3 py-3 text-content-200">
    <div
      class="flex h-8 w-8 min-w-8 items-center justify-center gap-1.5 rounded-full bg-content-600"
    >
      <SvgSpriteImg name="logo" viewBox="0 0 162 124" class="w-3/4" />
      <!-- System -->
    </div>

    <div class="flex-1 space-y-2">
      <i18n-t
        :keypath="`notification.tpl.${notification.type}`"
        tag="div"
        scope="global"
        class="pr-8 leading-loose"
      >
        <template #clan v-if="'clanId' in notification.activityLog.metadata">
          <div class="inline-flex items-center gap-1 align-middle">
            <UserClan :clan="getClanById(Number(notification.activityLog.metadata.clanId))!" />
          </div>
        </template>
        <template #user>
          <div class="inline-flex items-center gap-1 align-middle">
            <UserMedia :user="getUserById(notification.activityLog.userId)!" />
          </div>
        </template>

        <template #gold v-if="'gold' in notification.activityLog.metadata">
          <Coin
            :value="Number(notification.activityLog.metadata.gold)"
            data-aq-addLogItem-tpl-goldPrice
          />
        </template>

        <template #heirloomPoints v-if="'heirloomPoints' in notification.activityLog.metadata">
          <span
            class="inline-flex gap-1.5 align-text-bottom font-bold text-primary"
            data-aq-addLogItem-tpl-heirloomPoints
          >
            <OIcon icon="blacksmith" size="lg" />
            {{ $n(Number(notification.activityLog.metadata.heirloomPoints)) }}
          </span>
        </template>
      </i18n-t>

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
    </div>
  </div>
</template>
