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
  <div class="relative flex items-start gap-4 rounded-lg bg-base-200 px-3 py-3 text-content-200">
    <!-- TODO: -->
    <div
      class="flex h-8 w-8 min-w-8 items-center justify-center gap-1.5 rounded-full bg-content-600"
    >
      <SvgSpriteImg name="logo" viewBox="0 0 162 124" class="w-3/4" />
    </div>

    <!-- <UserMedia hiddenTitle hiddenPlatform :user="getUserById(notification.activityLog.userId)!" /> -->

    <div class="flex-1 space-y-3">
      <i18n-t
        :keypath="`notification.tpl.${notification.type}`"
        tag="div"
        scope="global"
        class="pr-8"
      >
        <template #clan v-if="'clanId' in notification.activityLog.metadata">
          <UserClan
            :clan="getClanById(Number(notification.activityLog.metadata.clanId))!"
            class="inline-flex items-center gap-1 align-middle"
          />
        </template>

        <template #user>
          <UserMedia
            :user="getUserById(notification.activityLog.userId)!"
            class="inline-flex items-center gap-1 align-middle"
          />
        </template>

        <template #actorUser>
          <UserMedia
            :user="getUserById(Number(notification.activityLog.metadata.actorUserId))!"
            class="inline-flex items-center gap-1 align-middle"
          />
        </template>

        <template #gold>
          <Coin
            v-if="'gold' in notification.activityLog.metadata"
            :value="Number(notification.activityLog.metadata.gold)"
          />
          <Coin
            v-if="'refundedGold' in notification.activityLog.metadata"
            :value="Number(notification.activityLog.metadata.refundedGold)"
          />
        </template>

        <template #heirloomPoints>
          <Loom
            v-if="'heirloomPoints' in notification.activityLog.metadata"
            :point="Number(notification.activityLog.metadata.heirloomPoints)"
          />
          <Loom
            v-if="'refundedHeirloomPoints' in notification.activityLog.metadata"
            :point="Number(notification.activityLog.metadata.refundedHeirloomPoints)"
          />
        </template>

        <template #itemId v-if="'itemId' in notification.activityLog.metadata">
          <strong>{{ notification.activityLog.metadata.itemId }}</strong>
        </template>

        <template
          #oldClanMemberRole
          v-if="'oldClanMemberRole' in notification.activityLog.metadata"
        >
          <strong>{{ notification.activityLog.metadata.oldClanMemberRole }}</strong>
        </template>

        <template
          #newClanMemberRole
          v-if="'newClanMemberRole' in notification.activityLog.metadata"
        >
          <strong>{{ notification.activityLog.metadata.newClanMemberRole }}</strong>
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
