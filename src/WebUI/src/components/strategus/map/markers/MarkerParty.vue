<script setup lang="ts">
import { LCircleMarker, LTooltip } from '@vue-leaflet/vue-leaflet';
import { strategusMaxPartyTroops, strategusMinPartyTroops } from '@root/data/constants.json';
import { type PartyCommon } from '@/models/strategus/party';
import { positionToLatLng } from '@/utils/geometry';

const { isSelf = false, party } = defineProps<{ party: PartyCommon; isSelf?: boolean }>();

const minRadius = 4;
const maxRadius = 10;

const markerRadius = computed(() => {
  const troopsRange = strategusMaxPartyTroops - strategusMinPartyTroops; // strategusMinPartyTroops = 300?
  const sizeFactor = party.troops / troopsRange;
  return minRadius + sizeFactor * (maxRadius - minRadius);
});

// TODO: clan mates
const markerColor = computed(() => (isSelf ? '#34d399' : '#ef4444')); // TODO: colors
</script>

<template>
  <LCircleMarker
    :latLng="positionToLatLng(party.position.coordinates)"
    :radius="markerRadius"
    :color="markerColor"
    :fillColor="markerColor"
    :fillOpacity="1.0"
    :bubblingMouseEvents="false"
  >
    <LTooltip :options="{ direction: 'top', offset: [0, -8] }">
      {{ party.user.name }} ({{ party.troops }})
    </LTooltip>
  </LCircleMarker>
</template>
