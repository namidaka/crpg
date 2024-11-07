<script setup lang="ts">
import { LIcon, LMarker } from '@vue-leaflet/vue-leaflet'

import { type SettlementPublic, SettlementType } from '~/models/strategus/settlement'
import { settlementIconByType } from '~/services/strategus-service/settlement'
import { positionToLatLng } from '~/utils/geometry'

const { settlement } = defineProps<{ settlement: SettlementPublic }>()

const settlementMarkerStyle = computed(() => {
  const output = {
    ...settlementIconByType[settlement.type],
    baseClass: '',
  }

  switch (settlement.type) {
    case SettlementType.Town:
      output.baseClass = 'text-sm px-2 py-1.5 gap-2'
      break
    case SettlementType.Castle:
      output.baseClass = 'text-xs px-1.5 py-1 gap-1.5'
      break

    case SettlementType.Village:
      output.baseClass = 'text-2xs px-1 py-1 gap-1'
      break
  }

  return output
})
</script>

<template>
  <LMarker
    :lat-lng="positionToLatLng(settlement.position.coordinates)"
    :options="{ bubblingMouseEvents: false }"
  >
    <LIcon class-name="!flex justify-center items-center">
      <div
        class="flex items-center whitespace-nowrap rounded-md bg-base-100 bg-opacity-30 text-white hover:ring"
        :class="settlementMarkerStyle.baseClass"
        :title="$t(`strategus.settlementType.${settlement.type}`)"
      >
        <OIcon
          :icon="settlementMarkerStyle.icon"
          :size="settlementMarkerStyle.iconSize"
          class="self-baseline"
        />
        <div class="leading-snug">
          {{ settlement.name }}
        </div>
      </div>
    </LIcon>
  </LMarker>
</template>
