<script setup lang="ts">
import { type Item } from '@/models/item';
import { useItem } from '@/composables/item/use-item';

const { item } = defineProps<{
  item: Item;
}>();

const { rankColor, thumb } = useItem(toRef(() => item));
</script>

<template>
  <article
    class="w-full items-center justify-center space-y-1 rounded-lg bg-base-200 p-1.5 ring ring-transparent hover:ring-border-200"
  >
    <div class="relative">
      <img :src="thumb" :alt="item.name" class="h-20 w-full select-none object-contain" />

      <div class="absolute left-0 top-0 z-10 flex items-center gap-1">
        <ItemRankIcon
          v-if="item.rank > 0"
          :rank="item.rank"
          class="cursor-default opacity-80 hover:opacity-100"
        />
        <slot name="badges-top-left" />
      </div>

      <div class="absolute right-0 top-0 z-10 flex items-center gap-1">
        <slot name="badges-top-right" />
      </div>

      <div class="absolute bottom-0 left-0 z-10 flex items-center gap-1">
        <slot name="badges-bottom-left" />
      </div>

      <div class="absolute bottom-0 right-0 z-10 flex items-center gap-1">
        <slot name="badges-bottom-right" />
      </div>
    </div>

    <h4 class="line-clamp-1 text-3xs font-bold" :style="{ color: rankColor }" v-tooltip="item.name">
      {{ item.name }}
    </h4>
  </article>
</template>
