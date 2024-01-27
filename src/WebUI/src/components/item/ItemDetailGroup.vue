<script setup lang="ts">
import { UseDraggable as Draggable } from '@vueuse/components';
import { useItemDetail } from '@/composables/character/use-item-detail';

const { openedItems, closeItemDetail, computeDetailCardYPosition } = useItemDetail(true);
</script>

<template>
  <Teleport to="body">
    <Draggable
      v-for="oi in openedItems"
      :key="oi.id"
      :initial-value="{
        x: oi.bound.x + oi.bound.width + 8,
        y: computeDetailCardYPosition(oi.bound.y),
      }"
      class="fixed z-50 cursor-move select-none overflow-hidden rounded-lg bg-base-300 shadow-lg active:ring-1"
    >
      <OButton
        class="!absolute right-2 top-2 z-10 cursor-pointer"
        iconRight="close"
        rounded
        size="2xs"
        variant="secondary"
        @click="closeItemDetail(oi)"
      />

      <slot v-bind="oi" />
    </Draggable>
  </Teleport>
</template>
