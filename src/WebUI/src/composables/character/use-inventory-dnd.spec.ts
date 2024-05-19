import { type PartialDeep } from 'type-fest';
import { createTestingPinia } from '@pinia/testing';

import { type UserItem, type UserItemsBySlot } from '@/models/user';
import { ItemSlot, ItemType } from '@/models/item';
import { type CharacterCharacteristics } from '@/models/character';
import { useUserStore } from '@/stores/user';

const userStore = useUserStore(createTestingPinia());

const {
  mockedEmit,
  mockedGetAvailableSlotsByItem,
  mockedGetLinkedSlots,
  mockedUseInventoryEquipment,
} = vi.hoisted(() => ({
  mockedEmit: vi.fn(),
  mockedGetAvailableSlotsByItem: vi.fn().mockReturnValue([]),
  mockedGetLinkedSlots: vi.fn().mockReturnValue([]),
  mockedUseInventoryEquipment: vi.fn().mockReturnValue({
    isEquipItemAllowed: vi.fn().mockReturnValue(true),
    getUnequipItemsLinked: vi.fn(),
  }),
}));

vi.mock('vue', async () => ({
  ...(await vi.importActual<typeof import('vue')>('vue')),
  getCurrentInstance: vi.fn().mockImplementation(() => ({
    emit: mockedEmit,
  })),
}));

vi.mock('@/services/item-service', () => ({
  getAvailableSlotsByItem: mockedGetAvailableSlotsByItem,
  getLinkedSlots: mockedGetLinkedSlots,
}));

vi.mock('@/composables/character/use-inventory-equipment', () => ({
  useInventoryEquipment: mockedUseInventoryEquipment,
}));

import { useInventoryDnD } from './use-inventory-dnd';

const userItemsBySlot: PartialDeep<UserItemsBySlot> = {
  [ItemSlot.Head]: {
    id: 4,
    item: {
      type: ItemType.HeadArmor,
    },
  },
  [ItemSlot.Weapon0]: {
    id: 3,
    item: {
      type: ItemType.OneHandedWeapon,
    },
  },
};

const characterCharacteristics: PartialDeep<CharacterCharacteristics> = {
  attributes: {
    strength: 12,
  },
};

const { isEquipItemAllowed, getUnequipItemsLinked } = mockedUseInventoryEquipment();

describe('useInventoryDnD', () => {
  describe('onDragStart', () => {
    it('no item', () => {
      const { focusedItemId, availableSlots, fromSlot, onDragStart, onDragEnd } = useInventoryDnD(
        ref(userItemsBySlot as UserItemsBySlot)
      );

      onDragStart(null, null);

      expect(focusedItemId.value).toEqual(null);
      expect(availableSlots.value).toEqual([]);
      expect(fromSlot.value).toEqual(null);

      onDragEnd(); // reset shared state
    });

    it('weapon: from inventory, to doll', () => {
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

      const { focusedItemId, availableSlots, fromSlot, onDragStart, onDragEnd } = useInventoryDnD(
        ref(userItemsBySlot as UserItemsBySlot)
      );

      onDragStart(userItem as UserItem, ItemSlot.Weapon1);

      expect(focusedItemId.value).toEqual(1);
      expect(availableSlots.value).toEqual(AVAILABLE_SLOTS);
      expect(fromSlot.value).toEqual(ItemSlot.Weapon1);

      onDragEnd();
    });

    it('weapon: from doll, to doll (another slot)', () => {
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

      const { focusedItemId, availableSlots, fromSlot, onDragStart, onDragEnd } = useInventoryDnD(
        ref(userItemsBySlot as UserItemsBySlot)
      );

      onDragStart(userItem as UserItem, null);

      expect(focusedItemId.value).toEqual(1);
      expect(availableSlots.value).toEqual(AVAILABLE_SLOTS);
      expect(fromSlot.value).toEqual(null);

      onDragEnd();
    });

    it('broken item', () => {
      userStore.$patch({ user: { id: 1 } });

      const userItem: PartialDeep<UserItem> = {
        id: 42,
        isBroken: true,
        item: { type: ItemType.HeadArmor, flags: [] },
      };

      const { focusedItemId, availableSlots, fromSlot, onDragStart, onDragEnd } = useInventoryDnD(
        ref(userItemsBySlot as UserItemsBySlot)
      );

      isEquipItemAllowed.mockReturnValueOnce(false);

      onDragStart(userItem as UserItem, ItemSlot.Head);

      expect(focusedItemId.value).toEqual(null);
      expect(availableSlots.value).toEqual([]);
      expect(fromSlot.value).toEqual(null);

      onDragEnd();
    });

    it('isArmoryItem item', () => {
      userStore.$patch({ user: { id: 1 } });

      const userItem: PartialDeep<UserItem> = {
        id: 42,
        userId: 1,
        isArmoryItem: true,
        item: { type: ItemType.HeadArmor, flags: [] },
      };

      const { focusedItemId, availableSlots, fromSlot, onDragStart, onDragEnd } = useInventoryDnD(
        ref(userItemsBySlot as UserItemsBySlot)
      );

      isEquipItemAllowed.mockReturnValueOnce(false);

      onDragStart(userItem as UserItem, ItemSlot.Head);

      expect(focusedItemId.value).toEqual(null);
      expect(availableSlots.value).toEqual([]);
      expect(fromSlot.value).toEqual(null);

      onDragEnd();
    });
  });

  it('onDragEnter', () => {
    const { toSlot, onDragEnter } = useInventoryDnD(ref(userItemsBySlot as UserItemsBySlot));

    expect(toSlot.value).toBeNull();

    onDragEnter(ItemSlot.Mount);

    expect(toSlot.value).toEqual(ItemSlot.Mount);
  });

  it('onDragLeave', () => {
    const { toSlot, onDragEnter, onDragLeave } = useInventoryDnD(
      ref(userItemsBySlot as UserItemsBySlot)
    );

    onDragEnter(ItemSlot.Mount);

    expect(toSlot.value).toEqual(ItemSlot.Mount);

    onDragLeave();

    expect(toSlot.value).toBeNull();
  });

  describe('onDragEnd', () => {
    it('empty slot', () => {
      const { toSlot, onDragEnter, onDragEnd } = useInventoryDnD(
        ref(userItemsBySlot as UserItemsBySlot)
      );

      onDragEnter(ItemSlot.Mount);

      expect(toSlot.value).toEqual(ItemSlot.Mount);

      onDragEnd();

      expect(mockedEmit).not.toBeCalled();
      expect(toSlot.value).toBeNull();
    });

    it('empty slot, with toSlot', () => {
      const { onDragEnd } = useInventoryDnD(ref(userItemsBySlot as UserItemsBySlot));

      onDragEnd();

      expect(mockedEmit).not.toBeCalled();
    });

    it('with slot, empty toSlot - drag item outside = unEquip item', () => {
      const { onDragEnd } = useInventoryDnD(ref(userItemsBySlot as UserItemsBySlot));

      getUnequipItemsLinked.mockReturnValueOnce([{ userItemId: null, slot: ItemSlot.Mount }]);

      onDragEnd(null, ItemSlot.Mount);

      expect(getUnequipItemsLinked).toBeCalledWith(ItemSlot.Mount, userItemsBySlot);
      expect(mockedEmit).toBeCalledWith('change', [{ userItemId: null, slot: ItemSlot.Mount }]);
    });

    it('unEquip item + unEquip linked items', () => {
      mockedGetLinkedSlots.mockReturnValueOnce([ItemSlot.Body]);
      const { onDragEnd } = useInventoryDnD(ref(userItemsBySlot as UserItemsBySlot));

      getUnequipItemsLinked.mockReturnValueOnce([
        { userItemId: null, slot: ItemSlot.Leg },
        { userItemId: null, slot: ItemSlot.Body },
      ]);

      onDragEnd(null, ItemSlot.Leg);

      expect(getUnequipItemsLinked).toBeCalledWith(ItemSlot.Leg, userItemsBySlot);
      expect(mockedEmit).toBeCalledWith('change', [
        { userItemId: null, slot: ItemSlot.Leg },
        { userItemId: null, slot: ItemSlot.Body },
      ]);
    });
  });

  describe('onDrop', () => {
    it('to empty, not available slots', () => {
      mockedGetAvailableSlotsByItem.mockReturnValue([ItemSlot.Mount]);

      const userItem: PartialDeep<UserItem> = {
        id: 1,
        item: { type: ItemType.Mount, flags: [] },
      };

      const { onDragStart, onDrop, onDragEnd } = useInventoryDnD(
        ref(userItemsBySlot as UserItemsBySlot)
      );

      onDragStart(userItem as UserItem);

      onDrop(ItemSlot.MountHarness);

      expect(mockedEmit).not.toBeCalled();

      onDragEnd();
    });

    it('to empty slot, available slot', () => {
      mockedGetAvailableSlotsByItem.mockReturnValue([ItemSlot.Mount]);

      const userItem: PartialDeep<UserItem> = {
        id: 1,
        item: { type: ItemType.Mount, flags: [] },
      };

      const { onDragStart, onDrop, onDragEnd } = useInventoryDnD(
        ref(userItemsBySlot as UserItemsBySlot)
      );

      onDragStart(userItem as UserItem);

      onDrop(ItemSlot.Mount);

      expect(mockedEmit).toBeCalledWith('change', [{ userItemId: 1, slot: ItemSlot.Mount }]);

      onDragEnd();
    });

    it('to full slot, available slot', () => {
      mockedGetAvailableSlotsByItem.mockReturnValue([ItemSlot.Head]);

      const userItem: PartialDeep<UserItem> = {
        id: 1,
        item: { type: ItemType.HeadArmor, flags: [] },
      };

      const { onDragStart, onDrop, onDragEnd } = useInventoryDnD(
        ref(userItemsBySlot as UserItemsBySlot)
      );

      onDragStart(userItem as UserItem);

      onDrop(ItemSlot.Head);

      expect(mockedEmit).toBeCalledWith('change', [{ userItemId: 1, slot: ItemSlot.Head }]);

      onDragEnd();
    });

    it('swap items - drop item from ItemSlot.Weapon1 to ItemSlot.Weapon0', () => {
      const AVAILABLE_SLOTS = [
        ItemSlot.Weapon0,
        ItemSlot.Weapon1,
        ItemSlot.Weapon2,
        ItemSlot.Weapon3,
      ];

      mockedGetAvailableSlotsByItem.mockReturnValue(AVAILABLE_SLOTS);

      const userItem: PartialDeep<UserItem> = {
        id: 2,
        item: { type: ItemType.TwoHandedWeapon, flags: [] },
      };

      const { onDragStart, onDrop, onDragEnd } = useInventoryDnD(
        ref({
          [ItemSlot.Weapon0]: {
            id: 1,
            item: {
              type: ItemType.OneHandedWeapon,
            },
          },
          [ItemSlot.Weapon1]: {
            id: 2,
            item: {
              type: ItemType.TwoHandedWeapon,
            },
          },
        } as UserItemsBySlot)
      );

      onDragStart(userItem as UserItem, ItemSlot.Weapon1);

      onDrop(ItemSlot.Weapon0);

      expect(mockedEmit).toBeCalledWith('change', [
        { userItemId: 2, slot: ItemSlot.Weapon0 },
        { userItemId: 1, slot: ItemSlot.Weapon1 },
      ]);

      onDragEnd();
    });
  });
});
