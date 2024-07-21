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
      <template #price v-if="'price' in notification.notification.metadata">
        <Coin
          :value="Number(notification.notification.metadata.price)"
          data-aq-addLogItem-tpl-goldPrice
        />
      </template>

      <template #gold v-if="'gold' in notification.notification.metadata">
        <Coin
          :value="Number(notification.notification.metadata.gold)"
          data-aq-addLogItem-tpl-goldPrice
        />
      </template>

      <template #heirloomPoints v-if="'heirloomPoints' in notification.notification.metadata">
        <span
          class="inline-flex gap-1.5 align-text-bottom font-bold text-primary"
          data-aq-addLogItem-tpl-heirloomPoints
        >
          <OIcon icon="blacksmith" size="lg" />
          {{ $n(Number(notification.notification.metadata.heirloomPoints)) }}
        </span>
      </template>
    </i18n-t>
  </div>
</template>
