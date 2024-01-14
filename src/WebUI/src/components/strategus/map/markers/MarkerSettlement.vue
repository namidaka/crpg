<script setup lang="ts">
import { clsx } from 'clsx';
import { LMarker, LTooltip, LIcon } from '@vue-leaflet/vue-leaflet';
import { SettlementType, type SettlementPublic } from '@/models/strategus/settlement';
import { positionToLatLng } from '@/utils/geometry';
import { settlementIconByType } from '@/services/strategus-service/settlement';
import { hexToRGBA } from '@/utils/color';

const { settlement } = defineProps<{ settlement: SettlementPublic }>();

const settlementMarkerStyle = computed(() => {
  const output = {
    ...settlementIconByType[settlement.type],
    baseClass: '',
    baseStyle: '',
  };

  switch (settlement.type) {
    case SettlementType.Town:
      output.baseClass = clsx('text-sm px-2 py-1.5 gap-2');
    case SettlementType.Castle:
      output.baseClass = clsx('text-xs px-1.5 py-1 gap-1.5');
    case SettlementType.Village:
      output.baseClass = clsx('text-2xs px-1 py-1 gap-1');
  }

  // if (settlement?.owner?.clan) {
  //   output.baseStyle = `background-color: ${hexToRGBA(
  //     settlement.owner?.clan?.clan?.primaryColor,
  //     0.3
  //   )};`;
  // }

  return output;
});
</script>

<template>
  <LMarker
    :latLng="positionToLatLng(settlement.position.coordinates)"
    :options="{ bubblingMouseEvents: false }"
  >
    <LIcon className="!flex justify-center items-center">
      <div
        :style="settlementMarkerStyle.baseStyle"
        class="flex items-center whitespace-nowrap rounded-md bg-base-100/50 text-white hover:ring"
        :class="settlementMarkerStyle.baseClass"
        :title="$t(`strategus.settlementType.${settlement.type}`)"
      >
        <OIcon :icon="settlementMarkerStyle.icon" :size="settlementMarkerStyle.iconSize" />
        <div class="leading-snug">{{ settlement.name }}</div>

        <div v-if="settlement?.owner?.clan" class="flex items-center">
          <!-- <ClanTagIcon :color="settlement.owner.clan.clan.primaryColor" size="xl" /> -->
          [{{ settlement.owner.clan.clan.tag }}]
        </div>
      </div>
    </LIcon>

    <!-- TODO: Settlement tooltip FIXME: -->
    <!-- <LTooltip :options="{ direction: 'top', offset: [0, -16] }">
      <div>
        <div class="flex min-w-[20rem] flex-col gap-2 p-2">
          <SettlementMedia :settlement="settlement!" />

          <div class="flex items-center gap-1.5" v-tooltip.bottom="`Troops`">
            <OIcon icon="member" size="lg" />
            {{ settlement!.troops }}
          </div>

          <Coin :value="10000" />

          <div v-if="settlement?.owner" class="flex flex-col gap-1">
            <span class="text-3xs text-content-300">Owner</span>
            <UserMedia :user="settlement.owner" />
          </div>
        </div>
      </div>
    </LTooltip> -->
  </LMarker>
</template>
