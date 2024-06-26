<script setup lang="ts">
import type { EquippedItemId } from '@/models/character';
import { ItemSlot } from '@/models/item';
import {
  getCharacterSLotsSchema,
  getOverallArmorValueBySlot,
  validateItemNotMeetRequirement,
} from '@/services/characters-service';
import { useInventoryDnD } from '@/composables/character/use-inventory-dnd';
import { useInventoryQuickEquip } from '@/composables/character/use-inventory-quick-equip';
import { useItemDetail } from '@/composables/character/use-item-detail';
import {
  equippedItemsBySlotKey,
  characterItemsStatsKey,
  characterCharacteristicsKey,
} from '@/symbols/character';

const equippedItemsBySlot = injectStrict(equippedItemsBySlotKey);
const itemsStats = injectStrict(characterItemsStatsKey);
const { characterCharacteristics } = injectStrict(characterCharacteristicsKey);

const slotsSchema = getCharacterSLotsSchema();

const emit = defineEmits<{
  (e: 'change', items: EquippedItemId[]): void;
  (e: 'sell', itemId: number): void;
}>();

const onUnEquipItem = (slot: ItemSlot) => {
  emit('change', [{ userItemId: null, slot }]);
};

const onClickInventoryDollSlot = (e: PointerEvent, slot: ItemSlot) => {
  if (equippedItemsBySlot.value[slot] === undefined) {
    return;
  }

  if (e.ctrlKey) {
    onQuickUnequip(slot);
  } else {
    toggleItemDetail(e.target as HTMLElement, {
      id: equippedItemsBySlot.value[slot].item.id,
      userItemId: equippedItemsBySlot.value[slot].id,
    });
  }
};

const {
  availableSlots,
  fromSlot,
  toSlot,
  onDragStart,
  onDragEnd,
  onDragEnter,
  onDragLeave,
  onDrop,
} = useInventoryDnD(equippedItemsBySlot);

const { onQuickUnequip } = useInventoryQuickEquip(equippedItemsBySlot);

const { toggleItemDetail } = useItemDetail();
</script>

<template>
  <div class="relative grid grid-cols-3 gap-4">
    <div class="absolute inset-0 -z-10 flex items-end justify-center">
      <SvgSpriteImg name="body-silhouette" viewBox="0 0 970 2200" class="w-52 2xl:w-64" />
    </div>
    <div
      v-for="(slotGroup, idx) in slotsSchema"
      class="flex flex-col gap-3"
      :class="[{ 'z-20': idx === 0 }, { 'z-10 justify-end': idx === 1 }]"
    >
      <CharacterInventoryDollSlot
        v-for="slot in slotGroup"
        :key="slot.key"
        :slot="slot.key"
        :placeholder="slot.placeholderIcon"
        :userItem="equippedItemsBySlot[slot.key]"
        :notMeetRequirement="
          slot.key in equippedItemsBySlot &&
          validateItemNotMeetRequirement(
            equippedItemsBySlot[slot.key].item,
            characterCharacteristics
          )
        "
        :available="Boolean(availableSlots.length && availableSlots.includes(slot.key))"
        :focused="toSlot === slot.key && availableSlots.includes(slot.key)"
        :armorOverall="getOverallArmorValueBySlot(slot.key, itemsStats)"
        :invalid="
          Boolean(
            availableSlots.length && toSlot === slot.key && !availableSlots.includes(slot.key)
          )
        "
        :remove="fromSlot === slot.key && !toSlot"
        draggable="true"
        @dragend="(_e: DragEvent) => onDragEnd(_e, slot.key)"
        @drop="onDrop(slot.key)"
        @dragover.prevent="onDragEnter(slot.key)"
        @dragleave.prevent="onDragLeave"
        @dragenter.prevent
        @dragstart="onDragStart(equippedItemsBySlot[slot.key], slot.key)"
        @unEquip="onUnEquipItem(slot.key)"
        @click="e => onClickInventoryDollSlot(e, slot.key)"
      />
    </div>
  </div>
</template>
