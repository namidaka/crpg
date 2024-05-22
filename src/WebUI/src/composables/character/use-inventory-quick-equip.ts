import { type EquippedItemId, type EquippedItemsBySlot } from '@/models/character';
import { ItemSlot } from '@/models/item';
import { type UserItem } from '@/models/user';
import { updateCharacterItems } from '@/services/characters-service';
import { getAvailableSlotsByItem, checkIsWeaponBySlot } from '@/services/item-service';
import { useUserStore } from '@/stores/user';
import { characterKey, characterItemsKey } from '@/symbols/character';
import { useInventoryEquipment } from '@/composables/character/use-inventory-equipment';

export const useInventoryQuickEquip = (equippedItemsBySlot: Ref<EquippedItemsBySlot>) => {
  const { user } = toRefs(useUserStore());
  const character = injectStrict(characterKey);
  const { loadCharacterItems } = injectStrict(characterItemsKey);
  const { isEquipItemAllowed, getUnequipItemsLinked } = useInventoryEquipment();

  const onQuickEquip = async (item: UserItem) => {
    if (!item || !isEquipItemAllowed(item, user.value!.id)) return;

    const availableSlots = getAvailableSlotsByItem(item.item, equippedItemsBySlot.value);
    const targetSlot = getTargetSlot(availableSlots);

    if (targetSlot) {
      const items: EquippedItemId[] = [{ userItemId: item.id, slot: targetSlot }];

      await updateItems(items);
    }
  };

  const onQuickUnequip = async (slot: ItemSlot) => {
    const items: EquippedItemId[] = getUnequipItemsLinked(slot, equippedItemsBySlot.value);

    await updateItems(items);
  };

  const updateItems = async (items: EquippedItemId[]) => {
    await updateCharacterItems(character.value.id, items);
    await loadCharacterItems(0, { id: character.value.id });
  };

  const getTargetSlot = (slots: ItemSlot[]): ItemSlot | undefined => {
    return slots
      .filter(slot => checkIsWeaponBySlot(slot) ? !equippedItemsBySlot.value[slot] : true)
      .at(0);
  };

  return {
    onQuickEquip,
    onQuickUnequip,
  };
};
