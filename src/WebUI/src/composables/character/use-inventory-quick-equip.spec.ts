import { type PartialDeep } from 'type-fest';
import { ItemSlot, ItemType } from '@/models/item';
import { type UserItemsBySlot, type UserItem } from '@/models/user';
import { useUserStore } from '@/stores/user';
import { createTestingPinia } from '@pinia/testing';
import { useInventoryQuickEquip } from './use-inventory-quick-equip';
import { characterItemsKey, characterKey } from '@/symbols/character';

const userStore = useUserStore(createTestingPinia());

const {
  mockedUseInventoryEquipment,
  mockedGetAvailableSlotsByItem,
  mockedUpdateCharacterItems,
  mockedInjectStrict,
} = vi.hoisted(() => ({
  mockedEmit: vi.fn(),
  mockedUseInventoryEquipment: vi.fn().mockReturnValue({
    isEquipItemAllowed: vi.fn().mockReturnValue(true),
    getUnequipItemsLinked: vi.fn(),
  }),
  mockedGetAvailableSlotsByItem: vi.fn(),
  mockedUpdateCharacterItems: vi.fn(),
  mockedLoadCharacterItems: vi.fn(),
  mockedInjectStrict: vi.fn().mockImplementation(key => {
    if (key === characterItemsKey) {
      return { loadCharacterItems: vi.fn() };
    } else if (key === characterKey) {
      return {
        value: {
          id: 1,
        },
      };
    }
  }),
}));

vi.mock('@/composables/character/use-inventory-equipment', () => ({
  useInventoryEquipment: mockedUseInventoryEquipment,
}));

vi.mock('@/services/item-service', () => ({
  weaponSlots: [ItemSlot.Weapon0, ItemSlot.Weapon1, ItemSlot.Weapon2, ItemSlot.Weapon3],
  getAvailableSlotsByItem: mockedGetAvailableSlotsByItem,
}));

vi.mock('@/services/characters-service', () => ({
  updateCharacterItems: mockedUpdateCharacterItems,
}));

vi.mock('@/utils/inject-strict/index', () => ({
  injectStrict: mockedInjectStrict,
}));

describe('useInventoryQuickEquip', () => {
  describe('onQuickEquip', () => {
    it('should update items', () => {
      userStore.$patch({ user: { id: 1 } });

      const userItem: PartialDeep<UserItem> = {
        id: 1,
        item: { type: ItemType.BodyArmor, flags: [] },
      };

      const AVAILABLE_SLOTS = [ItemSlot.Body];

      mockedGetAvailableSlotsByItem.mockReturnValue(AVAILABLE_SLOTS);

      const { onQuickEquip } = useInventoryQuickEquip(ref({} as UserItemsBySlot));

      onQuickEquip(userItem as UserItem);

      expect(mockedGetAvailableSlotsByItem).toBeCalledWith(userItem.item, {});
      expect(mockedUpdateCharacterItems).toBeCalledWith(1, [
        {
          userItemId: 1,
          slot: ItemSlot.Body,
        },
      ]);
    });

    it('should update non weapon items when slot already filled', () => {
      userStore.$patch({ user: { id: 1 } });

      const userItem: PartialDeep<UserItem> = {
        id: 1,
        item: { type: ItemType.BodyArmor, flags: [] },
      };

      const AVAILABLE_SLOTS = [ItemSlot.Body];

      mockedGetAvailableSlotsByItem.mockReturnValue(AVAILABLE_SLOTS);

      const userItemsBySlot: PartialDeep<UserItemsBySlot> = {
        [ItemSlot.Body]: {
          id: 2,
          item: {
            type: ItemType.BodyArmor,
          },
        },
      };

      const { onQuickEquip } = useInventoryQuickEquip(ref(userItemsBySlot as UserItemsBySlot));

      onQuickEquip(userItem as UserItem);

      expect(mockedGetAvailableSlotsByItem).toBeCalledWith(userItem.item, userItemsBySlot);
      expect(mockedUpdateCharacterItems).toBeCalledWith(1, [
        {
          userItemId: 1,
          slot: ItemSlot.Body,
        },
      ]);
    });

    it('should quick equip weapon to the next free slot', () => {
      userStore.$patch({ user: { id: 1 } });

      const userItem: PartialDeep<UserItem> = {
        id: 1,
        item: { type: ItemType.TwoHandedWeapon, flags: [] },
      };

      const AVAILABLE_SLOTS = [
        ItemSlot.Weapon0,
        ItemSlot.Weapon1,
        ItemSlot.Weapon2,
        ItemSlot.Weapon3,
      ];

      mockedGetAvailableSlotsByItem.mockReturnValue(AVAILABLE_SLOTS);

      const userItemsBySlot: PartialDeep<UserItemsBySlot> = {
        [ItemSlot.Weapon0]: {
          id: 2,
          item: {
            type: ItemType.OneHandedWeapon,
          },
        },
      };

      const { onQuickEquip } = useInventoryQuickEquip(ref(userItemsBySlot as UserItemsBySlot));

      onQuickEquip(userItem as UserItem);

      expect(mockedGetAvailableSlotsByItem).toBeCalledWith(userItem.item, userItemsBySlot);
      expect(mockedUpdateCharacterItems).toBeCalledWith(1, [
        {
          userItemId: 1,
          slot: ItemSlot.Weapon1,
        },
      ]);
    });

    it('should not equip weapon when all slots are full', () => {
      userStore.$patch({ user: { id: 1 } });

      const userItem: PartialDeep<UserItem> = {
        id: 1,
        item: { type: ItemType.TwoHandedWeapon, flags: [] },
      };

      const AVAILABLE_SLOTS = [
        ItemSlot.Weapon0,
        ItemSlot.Weapon1,
        ItemSlot.Weapon2,
        ItemSlot.Weapon3,
      ];

      mockedGetAvailableSlotsByItem.mockReturnValue(AVAILABLE_SLOTS);

      const userItemsBySlot: PartialDeep<UserItemsBySlot> = {
        [ItemSlot.Weapon0]: {
          id: 2,
          item: {
            type: ItemType.OneHandedWeapon,
          },
        },
        [ItemSlot.Weapon1]: {
          id: 3,
          item: {
            type: ItemType.Shield,
          },
        },
        [ItemSlot.Weapon2]: {
          id: 4,
          item: {
            type: ItemType.Bow,
          },
        },
        [ItemSlot.Weapon3]: {
          id: 5,
          item: {
            type: ItemType.Arrows,
          },
        },
      };

      const { onQuickEquip } = useInventoryQuickEquip(ref(userItemsBySlot as UserItemsBySlot));

      onQuickEquip(userItem as UserItem);

      expect(mockedGetAvailableSlotsByItem).toBeCalledWith(userItem.item, userItemsBySlot);
      expect(mockedUpdateCharacterItems).not.toBeCalled();
    });
  });

  describe('onQuickUnequip', () => {
    it('should update items', () => {
      const userItemsBySlot: PartialDeep<UserItemsBySlot> = {
        [ItemSlot.Weapon0]: {
          id: 1,
          item: {
            type: ItemType.OneHandedWeapon,
          },
        },
      };

      const { onQuickUnequip } = useInventoryQuickEquip(ref(userItemsBySlot as UserItemsBySlot));
      const { getUnequipItemsLinked } = mockedUseInventoryEquipment();

      getUnequipItemsLinked.mockReturnValueOnce([
        {
          userItemId: null,
          slot: ItemSlot.Weapon0,
        },
      ]);

      onQuickUnequip(ItemSlot.Weapon0);

      expect(getUnequipItemsLinked).toBeCalledWith(ItemSlot.Weapon0, userItemsBySlot);
      expect(mockedUpdateCharacterItems).toBeCalledWith(1, [
        {
          userItemId: null,
          slot: ItemSlot.Weapon0,
        },
      ]);
    });
  });
});
