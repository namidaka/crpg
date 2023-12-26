import { useAsyncState } from '@vueuse/core';
import {
  getClanArmory,
  addItemToClanArmory,
  removeItemFromClanArmory,
  returnItemToClanArmory,
  borrowItemFromClanArmory,
} from '@/services/clan-service';

export const useClanArmory = (clanId: number) => {
  const {
    state: clanArmory,
    execute: loadClanArmory,
    isLoading: isLoadingClanArmory,
  } = useAsyncState(() => getClanArmory(clanId), [], {
    immediate: false,
    resetOnExecute: false,
  });

  const addItem = (itemId: number) => addItemToClanArmory(clanId, itemId);

  const removeItem = (itemId: number) => removeItemFromClanArmory(clanId, itemId);

  const borrowItem = (itemId: number) => borrowItemFromClanArmory(clanId, itemId);

  const returnItem = (itemId: number) => returnItemToClanArmory(clanId, itemId);

  return {
    clanArmory,
    loadClanArmory,
    isLoadingClanArmory,
    addItem,
    removeItem,
    returnItem,
    borrowItem,
  };
};
