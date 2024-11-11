<script setup lang="ts">
defineProps<{
  color: string
}>()
</script>

<template>
  <OIcon
    icon="tag"
    size="sm"
    :style="{
      'color': '#fff',
      '--fa-primary-opacity': 0.15,
      '--fa-secondary-color': color,
    }"
  />
</template>
