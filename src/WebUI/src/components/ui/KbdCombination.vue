<script setup lang="ts">
defineProps<{
  label: string;
  keys: string[];
}>();
</script>

<template>
  <div class="flex items-center gap-2">
    <div class="flex items-center gap-2" v-for="(key, index) in keys">
      <Kbd>{{ key }}</Kbd>
      <span v-if="index + 1 !== keys.length" class="text-2xs">+</span>
    </div>
    <span class="text-2xs">{{ label }}</span>
  </div>
</template>
