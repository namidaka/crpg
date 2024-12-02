import type { EquippedItemsBySlot } from '~/models/character'
import type { ItemSlot } from '~/models/item'
import type { UserItem } from '~/models/user'

import { getLinkedSlots } from '~/services/item-service'
import { NotificationType, notify } from '~/services/notification-service'
import { t } from '~/services/translate-service'

export const useInventoryEquipment = () => {
  const isEquipItemAllowed = (item: UserItem, userId: number) => {
    if (item.isBroken) {
      notify(t('character.inventory.item.broken.notify.warning'), NotificationType.Warning)
      return false
    }

    if (item.isArmoryItem && userId === item.userId) {
      notify(
        t('character.inventory.item.clanArmory.inArmory.notify.warning'),
        NotificationType.Warning,
      )
      return false
    }

    return true
  }

  const getUnEquipItemsLinked = (slot: ItemSlot, equippedItemsBySlot: EquippedItemsBySlot) => {
    return [
      { slot, userItemId: null },
      ...getLinkedSlots(slot, equippedItemsBySlot).map(ls => ({
        slot: ls,
        userItemId: null,
      })),
    ]
  }

  return {
    getUnEquipItemsLinked,
    isEquipItemAllowed,
  }
}
