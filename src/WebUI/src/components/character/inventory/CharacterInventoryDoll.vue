<script setup lang="ts">
import { vOnLongPress } from '@vueuse/components'

import type { EquippedItemId } from '~/models/character'
import type { ItemSlot } from '~/models/item'

import { useInventoryDnD } from '~/composables/character/use-inventory-dnd'
import { useInventoryQuickEquip } from '~/composables/character/use-inventory-quick-equip'
import { useItemDetail } from '~/composables/character/use-item-detail'
import {
  getCharacterSLotsSchema,
  getOverallArmorValueBySlot,
  validateItemNotMeetRequirement,
} from '~/services/characters-service'
import {
  characterCharacteristicsKey,
  characterItemsStatsKey,
  equippedItemsBySlotKey,
} from '~/symbols/character'

defineEmits<{
  change: [items: EquippedItemId[]] // used in useInventoryDnD
}>()

const equippedItemsBySlot = injectStrict(equippedItemsBySlotKey)
const itemsStats = injectStrict(characterItemsStatsKey)
const { characterCharacteristics } = injectStrict(characterCharacteristicsKey)

const slotsSchema = getCharacterSLotsSchema()

const onClickInventoryDollSlot = (e: PointerEvent, slot: ItemSlot) => {
  if (equippedItemsBySlot.value[slot] === undefined) {
    return
  }

  if (e.ctrlKey) {
    onQuickUnEquip(slot)
  }
  else {
    toggleItemDetail(e.target as HTMLElement, {
      id: equippedItemsBySlot.value[slot].item.id,
      userItemId: equippedItemsBySlot.value[slot].id,
    })
  }
}

const {
  availableSlots,
  fromSlot,
  onDragEnd,
  onDragEnter,
  onDragLeave,
  onDragStart,
  onDrop,
  toSlot,
} = useInventoryDnD(equippedItemsBySlot)
const { onQuickUnEquip } = useInventoryQuickEquip(equippedItemsBySlot)

const { toggleItemDetail } = useItemDetail()
</script>

<template>
  <div class="relative grid grid-cols-3 gap-4">
    <div class="absolute inset-0 -z-10 flex items-end justify-center">
      <SvgSpriteImg
        name="body-silhouette"
        viewBox="0 0 970 2200"
        class="w-52 2xl:w-64"
      />
    </div>
    <div
      v-for="(slotGroup, idx) in slotsSchema"
      :key="idx"
      class="flex flex-col gap-3"
      :class="[{ 'z-20': idx === 0 }, { 'z-10 justify-end': idx === 1 }]"
    >
      <CharacterInventoryDollSlot
        v-for="slot in slotGroup"
        :key="slot.key"
        v-on-long-press="[
          () => {
            onQuickUnEquip(slot.key)
          },
          { delay: 500 },
        ]"
        :item-slot="slot.key"
        :placeholder="slot.placeholderIcon"
        :user-item="equippedItemsBySlot[slot.key]"
        :not-meet-requirement="
          slot.key in equippedItemsBySlot
            && validateItemNotMeetRequirement(
              equippedItemsBySlot[slot.key].item,
              characterCharacteristics,
            )
        "
        :available="Boolean(availableSlots.length && availableSlots.includes(slot.key))"
        :focused="toSlot === slot.key && availableSlots.includes(slot.key)"
        :armor-overall="getOverallArmorValueBySlot(slot.key, itemsStats)"
        :invalid="
          Boolean(
            availableSlots.length && toSlot === slot.key && !availableSlots.includes(slot.key),
          )
        "
        :remove="fromSlot === slot.key && !toSlot"
        draggable="true"
        @dragstart="onDragStart(equippedItemsBySlot[slot.key], slot.key)"
        @dragend="(_e: DragEvent) => onDragEnd(_e, slot.key)"
        @drop="onDrop(slot.key)"
        @dragover.prevent="onDragEnter(slot.key)"
        @dragleave.prevent="onDragLeave"
        @dragenter.prevent
        @click="e => onClickInventoryDollSlot(e, slot.key)"
      />
    </div>
  </div>
</template>
