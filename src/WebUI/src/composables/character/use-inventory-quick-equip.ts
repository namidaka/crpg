import { EquippedItemId, EquippedItemsBySlot } from '@/models/character';
import { ItemSlot } from '@/models/item';
import { UserItem } from '@/models/user';
import { updateCharacterItems } from '@/services/characters-service';
import { getAvailableSlotsByItem, getLinkedSlots } from '@/services/item-service';
import { NotificationType, notify } from '@/services/notification-service';
import { t } from '@/services/translate-service';
import { useUserStore } from '@/stores/user';
import { characterKey, characterItemsKey } from '@/symbols/character';

export const useInventoryQuickEquip = (equippedItemsBySlot: Ref<EquippedItemsBySlot>) => {
  const { user } = toRefs(useUserStore());
  const character = injectStrict(characterKey);
  const { loadCharacterItems } = injectStrict(characterItemsKey);

  const onQuickEquip = async (item: UserItem) => {
    if (!item) return;

    if (item.isBroken) {
      notify(t('character.inventory.item.broken.notify.warning'), NotificationType.Warning);
      return;
    }

    if (item.isArmoryItem && user.value!.id === item.userId) {
      notify(
        t('character.inventory.item.clanArmory.inArmory.notify.warning'),
        NotificationType.Warning
      );
      return;
    }

    const availableSlots = getAvailableSlotsByItem(item.item, equippedItemsBySlot.value);
    const targetSlot = getTargetSlot(availableSlots);

    if (targetSlot) {
      const items: EquippedItemId[] = [{ userItemId: item.id, slot: targetSlot }];

      await updateItems(items);
    }
  };

  const onQuickUnequip = async (slot: ItemSlot) => {
    const items: EquippedItemId[] = [
      { userItemId: null, slot },
      ...getLinkedSlots(slot, equippedItemsBySlot.value).map(ls => ({
        userItemId: null,
        slot: ls,
      })),
    ];

    await updateItems(items);
  };

  const updateItems = async (items: EquippedItemId[]) => {
    await updateCharacterItems(character.value.id, items);
    await loadCharacterItems(0, { id: character.value.id });
  };

  const getTargetSlot = (slots: ItemSlot[]): ItemSlot | undefined => {
    return slots
      .filter(slot => isMainWeaponSlot(slot) ? !equippedItemsBySlot.value[slot] : true)
      .at(0);
  };

  const isMainWeaponSlot = (slot: ItemSlot): boolean => {
    return /^Weapon[0-9]{1}$/.test(slot);
  };

  return {
    onQuickEquip,
    onQuickUnequip,
  };
};
