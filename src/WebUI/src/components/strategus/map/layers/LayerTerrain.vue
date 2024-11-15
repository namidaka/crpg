<script setup lang="ts">
import type L from 'leaflet'

import { LGeoJson } from '@vue-leaflet/vue-leaflet'

import type { TerrainFeature, TerrainFeatureCollection } from '~/models/strategus/terrain'

import { TerrainColorByType } from '~/services/strategus-service/terrain'

const { data } = defineProps<{
  data: TerrainFeatureCollection
}>()

const emit = defineEmits<{
  edit: [e: L.PM.EditEventHandler]
}>()

const terrainGeoJSONStyle = (feature: TerrainFeature) => ({
  color: TerrainColorByType[feature.properties.type],
})

const onEachFeatureFunction = (feature: TerrainFeature, layer: L.Polygon) => {
  // @ts-expect-error TODO:
  layer.internalId = feature.id
  // @ts-expect-error TODO:

  layer.properties = feature.properties

  // @ts-expect-error TODO:
  layer.on('pm:edit', e => emit('edit', e))
}

const terrainGeoJSONOptions = {
  onEachFeature: onEachFeatureFunction,
}
</script>

<template>
  <!-- TODO: ts -->
  <LGeoJson
    :geojson="data"
    :options-style="terrainGeoJSONStyle"
    :options="terrainGeoJSONOptions"
  />
</template>
