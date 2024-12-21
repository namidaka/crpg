<script setup lang="ts">
const { value } = defineProps<{ value?: number }>()
</script>

<template>
  <span class="inline-flex gap-1.5 align-text-bottom font-bold text-primary">
    <SvgSpriteImg
      name="coin"
      viewBox="0 0 18 18"
      class="w-4"
    />
    <slot v-if="$slots.default" />
    <template v-else-if="value !== undefined">{{ $n(value) }}</template>
  </span>
</template>
