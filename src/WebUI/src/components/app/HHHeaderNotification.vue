<script setup lang="ts">
import VueCountdown from '@chenfengyuan/vue-countdown';
import { useHappyHours } from '@/composables/use-hh';
import { useUserStore } from '@/stores/user';

const userStore = useUserStore();

const { HHEventRemaining, onStartHHCountdown, onEndHHCountdown, transformSlotProps } =
  useHappyHours();
</script>

<template>
  <div class="flex items-center justify-center bg-status-success px-8 py-1">
    <HHTooltip :region="userStore.user!.region">
      <div class="flex-1 cursor-pointer items-center gap-2 text-sm text-content-100">
        🎉
        <VueCountdown
          class="w-24"
          :time="HHEventRemaining"
          :transform="transformSlotProps"
          v-slot="{ hours, minutes, seconds }"
          @start="onStartHHCountdown"
          @end="onEndHHCountdown"
        >
          {{ $t('dateTimeFormat.countdown', { hours, minutes, seconds }) }}
        </VueCountdown>
        🎉
      </div>
    </HHTooltip>
  </div>
</template>
