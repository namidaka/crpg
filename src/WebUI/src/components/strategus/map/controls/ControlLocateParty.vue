<script setup lang="ts">
import type { Map } from 'leaflet'

import { LControl } from '@vue-leaflet/vue-leaflet'

import type { Party } from '~/models/strategus/party'

import { positionToLatLng } from '~/utils/geometry'

const { party } = defineProps<{ party: Party }>()

const map = ref<Map | null>(null)

const onReady = (leafletObject: typeof LControl) => {
  map.value = leafletObject._map as Map
}

const onClick = () => {
  map.value!.flyTo(positionToLatLng(party.position.coordinates))
}
</script>

<template>
  <LControl @ready="onReady">
    <div class="leaflet-bar">
      <a
        href="#"
        class="!flex items-center justify-center"
        :title="$t('strategus.control.locateParty')"
        @click="onClick"
      >
        <OIcon
          icon="crosshair"
          size="lg"
        />
      </a>
    </div>
  </LControl>
</template>
