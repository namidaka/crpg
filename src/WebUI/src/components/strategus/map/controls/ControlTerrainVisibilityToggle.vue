<script setup lang="ts">
import { LControl } from '@vue-leaflet/vue-leaflet';
</script>

<template>
  <LControl>
    <div class="leaflet-bar">
      <a
        href="#"
        class="!flex items-center justify-center"
        :title="$t('strategus.control.terrainLayerToggle')"
      >
        <OIcon icon="layers" size="lg" />
      </a>
    </div>
  </LControl>
</template>
