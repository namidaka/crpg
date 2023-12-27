<script setup lang="ts">
const { value = 0 } = defineProps<{ value?: number }>();
</script>

<template>
  <span class="flex items-center gap-1.5 font-bold text-primary">
    <OIcon icon="blacksmith" size="lg" />
    {{ value }}
  </span>
</template>
