<script setup lang="ts">
const { title } = defineProps<{ title: string }>()
</script>

<template>
  <div class="item-center mb-14 flex select-none justify-center gap-4 md:gap-8">
    <SvgSpriteImg
      name="logo-decor"
      viewBox="0 0 108 10"
      class="w-16 rotate-180 md:w-28"
    />
    <h1 class="text-center text-2xl text-content-100">
      {{ title }}
    </h1>
    <SvgSpriteImg
      name="logo-decor"
      viewBox="0 0 108 10"
      class="w-16 md:w-28"
    />
  </div>
</template>
