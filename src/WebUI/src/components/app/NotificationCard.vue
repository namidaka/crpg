<script setup lang="ts">
import { type ActivityLog, ActivityLogType } from '@/models/activity-logs';
import { NotificationState } from '@/models/notificatios';
import { UserNotification } from '@/models/user';
import { getItemImage } from '@/services/item-service';

const { notification } = defineProps<{
  notification: UserNotification;
}>();
</script>

<template>
  <div class="flex flex-col space-y-2 bg-base-200 px-5 py-3 text-content-200">
    <div class="flex items-center gap-2">
      <!-- TODO: create System User cmp -->
      <div class="flex items-center gap-1.5 text-content-100">
        <SvgSpriteImg name="logo" viewBox="0 0 162 124" class="w-6" />
        System
      </div>

      <div class="text-2xs text-content-300">
        {{ $d(new Date(notification.notification.createdAt), 'short') }}
      </div>

      <Tag variant="primary" :label="notification.notification.type" />
    </div>

    <div>{{ notification.state }}</div>

    <i18n-t
      :keypath="`notification.tpl.${notification.notification.type}`"
      tag="div"
      scope="global"
    >
      <!-- <template #price v-if="'price' in activityLog.metadata">
        <Coin :value="Number(activityLog.metadata.price)" data-aq-addLogItem-tpl-goldPrice />
      </template>

      <template #gold v-if="'gold' in activityLog.metadata">
        <Coin :value="Number(activityLog.metadata.gold)" data-aq-addLogItem-tpl-goldPrice />
      </template> -->

      <template #heirloomPoints v-if="'heirloomPoints' in notification.notification.metadata">
        <span
          class="inline-flex gap-1.5 align-text-bottom font-bold text-primary"
          data-aq-addLogItem-tpl-heirloomPoints
        >
          <OIcon icon="blacksmith" size="lg" />
          {{ $n(Number(notification.notification.metadata.heirloomPoints)) }}
        </span>
      </template>

      <!-- <template #itemId v-if="'itemId' in activityLog.metadata">
        <span class="inline" data-aq-addLogItem-tpl-itemId>
          <VTooltip placement="auto" class="inline-block">
            <span class="font-bold text-content-100">{{ activityLog.metadata.itemId }}</span>
            <template #popper>
              <img
                :src="getItemImage(activityLog.metadata.itemId)"
                class="h-full w-full object-contain"
              />
            </template>
          </VTooltip>
        </span>
      </template>

      <template #experience v-if="'experience' in activityLog.metadata">
        <span class="font-bold text-content-100" data-aq-addLogItem-tpl-experience>
          {{ $n(Number(activityLog.metadata.experience)) }}
        </span>
      </template>

      <template #damage v-if="'damage' in activityLog.metadata">
        <span class="font-bold text-status-danger" data-aq-addLogItem-tpl-damage>
          {{ $n(Number(activityLog.metadata.damage)) }}
        </span>
      </template>

      <template #targetUserId v-if="Number(activityLog.metadata.targetUserId) in users">
        <div
          class="inline-flex items-center gap-1 align-middle"
          data-aq-addLogItem-tpl-targetUserId
        >
          <RouterLink
            :to="{
              name: 'ModeratorUserIdRestrictions',
              params: { id: activityLog.metadata.targetUserId },
            }"
            class="inline-block hover:text-content-100"
            target="_blank"
          >
            <UserMedia :user="users[Number(activityLog.metadata.targetUserId)]" />
          </RouterLink>
          <OButton
            v-if="isSelfUser"
            size="2xs"
            iconLeft="add"
            rounded
            variant="secondary"
            data-aq-addLogItem-addUser-btn
            @click="emit('addUser', Number(activityLog.metadata.targetUserId))"
          />
        </div>
      </template>

      <template #actorUserId v-if="'actorUserId' in activityLog.metadata">
        <div class="inline-flex items-center gap-1 align-middle">
          <RouterLink
            class="inline-block hover:text-content-100"
            :to="{
              name: 'ModeratorUserIdInformation',
              params: { id: activityLog.metadata.actorUserId },
            }"
            target="_blank"
          >
            <UserMedia
              :user="users[Number(activityLog.metadata.actorUserId)]"
              hiddenClan
              hiddenPlatform
            />
          </RouterLink>
        </div>
      </template>

      <template #instance v-if="'instance' in activityLog.metadata">
        <Tag variant="info" :label="activityLog.metadata.instance" />
      </template>

      <template #gameMode v-if="'gameMode' in activityLog.metadata">
        <Tag variant="info" :label="activityLog.metadata.gameMode" />
      </template>

      <template #oldName v-if="'oldName' in activityLog.metadata">
        <span class="font-bold text-content-100">{{ activityLog.metadata.oldName }}</span>
      </template>

      <template #newName v-if="'newName' in activityLog.metadata">
        <span class="font-bold text-content-100">{{ activityLog.metadata.newName }}</span>
      </template>

      <template #characterId v-if="'characterId' in activityLog.metadata">
        <span class="font-bold text-content-100">{{ activityLog.metadata.characterId }}</span>
      </template>

      <template #generation v-if="'generation' in activityLog.metadata">
        <span class="font-bold text-content-100">{{ activityLog.metadata.generation }}</span>
      </template>

      <template #level v-if="'level' in activityLog.metadata">
        <span class="font-bold text-content-100">{{ activityLog.metadata.level }}</span>
      </template>

      <template #message v-if="'message' in activityLog.metadata">
        <span class="font-bold text-content-100">{{ activityLog.metadata.message }}</span>
      </template>

      <template #clanId v-if="'clanId' in activityLog.metadata">
        <span class="font-bold text-content-100">{{ activityLog.metadata.clanId }}</span>
      </template>

      <template #userItemId v-if="'userItemId' in activityLog.metadata">
        <span class="font-bold text-content-100">{{ activityLog.metadata.userItemId }}</span>
      </template> -->
    </i18n-t>
  </div>
</template>
