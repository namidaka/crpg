<script setup lang="ts">
defineProps<{
  label: string
  keys: string[]
}>()
</script>

<template>
  <div class="flex items-center gap-2">
    <div class="flex gap-2">
      <Kbd v-for="key in keys" :key="key">{{ key }}</Kbd>
    </div>
    <span class="text-2xs">{{ label }}</span>
  </div>
</template>
