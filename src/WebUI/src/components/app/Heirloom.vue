<script setup lang="ts">
const { value = 0 } = defineProps<{ value?: number }>()
</script>

<template>
  <span class="inline-flex items-center gap-1.5 font-bold text-primary">
    <OIcon
      icon="blacksmith"
      size="lg"
      class="text-primary"
    />
    {{ value }}
  </span>
</template>
