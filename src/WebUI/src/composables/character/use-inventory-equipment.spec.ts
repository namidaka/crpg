import { ItemFamilyType, ItemSlot, ItemType } from '@/models/item';
import { type UserItem } from '@/models/user';
import { type PartialDeep } from 'type-fest';
import { useInventoryEquipment } from './use-inventory-equipment';
import { type EquippedItemsBySlot } from '@/models/character';

const { mockedNotify, mockedGetLinkedSlots } = vi.hoisted(() => ({
  mockedNotify: vi.fn(),
  mockedGetLinkedSlots: vi.fn().mockReturnValue([]),
}));

vi.mock('@/services/notification-service', async () => ({
  ...(await vi.importActual<typeof import('@/services/notification-service')>(
    '@/services/notification-service'
  )),
  notify: mockedNotify,
}));

vi.mock('@/services/item-service', () => ({
  getLinkedSlots: mockedGetLinkedSlots,
}));

describe('useInventoryEquipment', () => {
  describe('isEquipItemAllowed', () => {
    it('broken item', () => {
      const userItem: PartialDeep<UserItem> = {
        id: 42,
        isBroken: true,
        item: { type: ItemType.HeadArmor, flags: [] },
      };

      const { isEquipItemAllowed } = useInventoryEquipment();

      const result = isEquipItemAllowed(userItem as UserItem, 1);

      expect(mockedNotify).toBeCalledWith(
        'character.inventory.item.broken.notify.warning',
        'warning'
      );
      expect(result).toBe(false);
    });

    it('item in armory', () => {
      const userItem: PartialDeep<UserItem> = {
        id: 42,
        userId: 1,
        isArmoryItem: true,
        item: { type: ItemType.HeadArmor, flags: [] },
      };

      const { isEquipItemAllowed } = useInventoryEquipment();

      const result = isEquipItemAllowed(userItem as UserItem, 1);

      expect(mockedNotify).toBeCalledWith(
        'character.inventory.item.clanArmory.inArmory.notify.warning',
        'warning'
      );
      expect(result).toBe(false);
    });

    it('item should be allowed', () => {
      const userItem: PartialDeep<UserItem> = {
        id: 42,
        item: { type: ItemType.HeadArmor, flags: [] },
      };

      const { isEquipItemAllowed } = useInventoryEquipment();

      const result = isEquipItemAllowed(userItem as UserItem, 1);

      expect(result).toBe(true);
    });
  });

  describe('getUnequipItemsLinked', () => {
    it('returns unequip items correctly', () => {
      const equippedItemsBySlot: PartialDeep<EquippedItemsBySlot> = {
        [ItemSlot.Body]: {
          id: 4,
          item: {
            type: ItemType.BodyArmor,
            armor: {
              familyType: ItemFamilyType.EBA,
            }
          },
        },
        [ItemSlot.Leg]: {
          id: 3,
          item: {
            type: ItemType.LegArmor,
            armor: {
              familyType: ItemFamilyType.EBA,
            }
          },
        },
      };

      mockedGetLinkedSlots.mockReturnValueOnce([ItemSlot.Body]);
      const { getUnequipItemsLinked } = useInventoryEquipment();

      const result = getUnequipItemsLinked(ItemSlot.Leg, equippedItemsBySlot as EquippedItemsBySlot);

      expect(mockedGetLinkedSlots).toBeCalledWith(ItemSlot.Leg, equippedItemsBySlot as EquippedItemsBySlot);
      expect(result).toStrictEqual([
        { userItemId: null, slot: ItemSlot.Leg },
        { userItemId: null, slot: ItemSlot.Body },
      ]);
    });
  });
});
