<script setup lang="ts">
const { active = false } = defineProps<{ active?: boolean }>()
</script>

<template>
  <OButton
    variant="secondary"
    outlined
    size="xl"
    rounded
  >
    <FontAwesomeLayers
      full-width
      class="fa-2x"
    >
      <FontAwesomeIcon :icon="['crpg', 'dots']" />
      <FontAwesomeLayersText
        v-if="active"
        counter
        value="●"
        position="top-right"
        :style="{ '--fa-counter-background-color': 'rgba(83, 188, 150, 1)' }"
      />
    </FontAwesomeLayers>
  </OButton>
</template>
