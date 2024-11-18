import type { EquippedItemId, EquippedItemsBySlot } from '~/models/character'
import type { ItemSlot } from '~/models/item'
import type { UserItem } from '~/models/user'

import { useInventoryEquipment } from '~/composables/character/use-inventory-equipment'
import { getAvailableSlotsByItem } from '~/services/item-service'
import { useUserStore } from '~/stores/user'

// Shared state
const focusedItemId = ref<number | null>(null)
const availableSlots = ref<ItemSlot[]>([])
const fromSlot = ref<ItemSlot | null>(null)
const toSlot = ref<ItemSlot | null>(null)

export const useInventoryDnD = (equippedItemsBySlot: Ref<EquippedItemsBySlot>) => {
  const { user } = toRefs(useUserStore())
  const { emit } = getCurrentInstance() as NonNullable<ReturnType<typeof getCurrentInstance>>
  const { getUnequipItemsLinked, isEquipItemAllowed } = useInventoryEquipment()

  const onDragStart = (item: UserItem | null = null, slot: ItemSlot | null = null) => {
    if (!item || !isEquipItemAllowed(item, user.value!.id)) { return }

    focusedItemId.value = item.id
    availableSlots.value = getAvailableSlotsByItem(item.item, equippedItemsBySlot.value)

    if (slot) {
      fromSlot.value = slot
    }
  }

  const onDragEnter = (slot: ItemSlot) => {
    toSlot.value = slot
  }

  const onDragLeave = () => {
    toSlot.value = null
  }

  const onDragEnd = (_e: DragEvent | null = null, slot: ItemSlot | null = null) => {
    if (slot && !toSlot.value) {
      const items: EquippedItemId[] = getUnequipItemsLinked(slot, equippedItemsBySlot.value)

      emit('change', items) // drop outside
    }

    focusedItemId.value = null
    availableSlots.value = []
    fromSlot.value = null
    toSlot.value = null
  }

  const onDrop = (slot: ItemSlot) => {
    if (!availableSlots.value.includes(slot)) { return }

    const items: EquippedItemId[] = [{ slot, userItemId: focusedItemId.value }]

    // switch items (weapon)
    if (fromSlot.value) {
      items.push({
        slot: fromSlot.value,
        userItemId:
          equippedItemsBySlot.value[slot] !== undefined ? equippedItemsBySlot.value[slot].id : null,
      })
    }

    emit('change', items)
  }

  return {
    availableSlots: readonly(availableSlots),
    focusedItemId: readonly(focusedItemId),
    fromSlot: readonly(fromSlot),
    onDragEnd,
    onDragEnter,
    onDragLeave,
    onDragStart,
    onDrop,
    toSlot: readonly(toSlot),
  }
}
