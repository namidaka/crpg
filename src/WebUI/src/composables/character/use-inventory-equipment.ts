import { type EquippedItemsBySlot } from '@/models/character';
import { ItemSlot } from '@/models/item';
import { type UserItem } from '@/models/user';
import { getLinkedSlots } from '@/services/item-service';
import { NotificationType, notify } from '@/services/notification-service';
import { t } from '@/services/translate-service';

export const useInventoryEquipment = () => {
  const isEquipItemAllowed = (item: UserItem, userId: number) => {
    if (item.isBroken) {
      notify(t('character.inventory.item.broken.notify.warning'), NotificationType.Warning);
      return false;
    }

    if (item.isArmoryItem && userId === item.userId) {
      notify(
        t('character.inventory.item.clanArmory.inArmory.notify.warning'),
        NotificationType.Warning
      );
      return false;
    }

    return true;
  };

  const getUnequipItemsLinked = (slot: ItemSlot, equippedItemsBySlot: EquippedItemsBySlot) => {
    return [
      { userItemId: null, slot },
      ...getLinkedSlots(slot, equippedItemsBySlot).map(ls => ({
        userItemId: null,
        slot: ls,
      })),
    ];
  };

  return {
    isEquipItemAllowed,
    getUnequipItemsLinked,
  };
};
