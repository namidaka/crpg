<script setup lang="ts">
import L from 'leaflet';
import { LGeoJson } from '@vue-leaflet/vue-leaflet';
import { type TerrainFeature, type TerrainFeatureCollection } from '@/models/strategus/terrain';
import { TerrainColorByType } from '@/services/strategus-service/terrain';

const { data } = defineProps<{
  data: TerrainFeatureCollection;
}>();

const emit = defineEmits<{
  update: [e: L.PM.UpdateEventHandler];
}>();

const terrainGeoJSONStyle = (feature: TerrainFeature) => ({
  color: TerrainColorByType[feature.properties.type],
});

const onEachFeatureFunction = (feature: TerrainFeature, layer: L.Polygon) => {
  // @ts-ignore
  layer.internalId = feature.id;
  // @ts-ignore
  layer.properties = feature.properties;

  // @ts-ignore TODO:
  layer.on('pm:update', e => emit('update', e));
};

const terrainGeoJSONOptions = {
  onEachFeature: onEachFeatureFunction,
};
</script>

<template>
  <!-- TODO: ts -->
  <LGeoJson :geojson="data" :optionsStyle="terrainGeoJSONStyle" :options="terrainGeoJSONOptions" />
</template>
