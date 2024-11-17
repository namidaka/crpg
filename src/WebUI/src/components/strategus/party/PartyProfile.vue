<script setup lang="ts">
import { type Party } from '@/models/strategus/party';

const { party } = defineProps<{ party: Party }>();

defineEmits<{
  locate: [];
}>();
</script>

<template>
  <div
    class="w-[22rem] rounded-3xl bg-base-100 bg-opacity-90 p-6 text-content-200 backdrop-blur-sm"
  >
    <div class="flex flex-col gap-2">
      <div class="flex items-center gap-2 self-center">
        <div v-tooltip.bottom="party.position.coordinates.join(' ')" @click="$emit('locate')">
          <OIcon icon="crosshair" size="lg" class="cursor-pointer" />
        </div>

        <UserMedia
          :user="party.user"
          :clan="party.clan?.clan"
          :clanRole="party.clan?.role"
          hiddenPlatform
          class="max-w-[12rem]"
        />
      </div>

      <Divider class="text-content-500" />

      <div class="flex items-center gap-1.5">
        <Coin :value="10000" />
        <Media icon="member" :label="String(party.troops)" />
      </div>

      <div>Status: {{ party.status }}</div>

      <div>Terrain: Plain</div>
    </div>
  </div>
</template>
