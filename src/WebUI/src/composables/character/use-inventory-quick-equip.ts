import type { EquippedItemId, EquippedItemsBySlot } from '~/models/character'
import type { ItemSlot } from '~/models/item'
import type { UserItem } from '~/models/user'

import { useInventoryEquipment } from '~/composables/character/use-inventory-equipment'
import { updateCharacterItems } from '~/services/characters-service'
import { checkIsWeaponBySlot, getAvailableSlotsByItem } from '~/services/item-service'
import { useUserStore } from '~/stores/user'
import { characterItemsKey, characterKey } from '~/symbols/character'

export const useInventoryQuickEquip = (equippedItemsBySlot: Ref<EquippedItemsBySlot>) => {
  const { user } = toRefs(useUserStore())
  const character = injectStrict(characterKey)
  const { loadCharacterItems } = injectStrict(characterItemsKey)
  const { getUnequipItemsLinked, isEquipItemAllowed } = useInventoryEquipment()

  const onQuickEquip = async (item: UserItem) => {
    if (!item || !isEquipItemAllowed(item, user.value!.id)) { return }

    const availableSlots = getAvailableSlotsByItem(item.item, equippedItemsBySlot.value)
    const targetSlot = getTargetSlot(availableSlots)

    if (targetSlot) {
      const items: EquippedItemId[] = [{ slot: targetSlot, userItemId: item.id }]

      await updateItems(items)
    }
  }

  const onQuickUnequip = async (slot: ItemSlot) => {
    const items: EquippedItemId[] = getUnequipItemsLinked(slot, equippedItemsBySlot.value)

    await updateItems(items)
  }

  const updateItems = async (items: EquippedItemId[]) => {
    await updateCharacterItems(character.value.id, items)
    await loadCharacterItems(0, { id: character.value.id })
  }

  const getTargetSlot = (slots: ItemSlot[]): ItemSlot | undefined => {
    return slots
      .filter(slot => checkIsWeaponBySlot(slot) ? !equippedItemsBySlot.value[slot] : true)
      .at(0)
  }

  return {
    onQuickEquip,
    onQuickUnequip,
  }
}
