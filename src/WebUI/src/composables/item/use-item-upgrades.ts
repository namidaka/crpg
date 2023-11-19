import { type ItemFlat } from '@/models/item';
import { type AggregationConfig } from '@/models/item-search';
import { getItemUpgrades, getRelativeEntries } from '@/services/item-service';
import { clamp } from '@/utils/math';
import { useUserStore } from '@/stores/user';

export const useItemUpgrades = (item: ItemFlat, cols: AggregationConfig) => {
  const userStore = useUserStore();

  const { state: itemUpgrades, isLoading } = useAsyncState(() => getItemUpgrades(item), []);

  const baseItem = computed(() => itemUpgrades.value.find(iu => iu.rank === 0)!);

  const nextItem = computed(
    () => itemUpgrades.value[clamp(itemUpgrades.value.findIndex(iu => iu.id === item.id) + 1, 0, 3)]
  );

  const relativeEntries = computed(() => getRelativeEntries(baseItem.value, cols));

  const validation = computed(() => ({
    points: userStore.user!.heirloomPoints > 0,
    maxRank: item.rank !== 3,
    exist: !userStore.userItems.some(
      ui => ui.item.baseId === nextItem.value?.baseId && ui.item.rank === nextItem.value?.rank
    ),
  }));

  const canUpgrade = computed(
    () => validation.value.points && validation.value.maxRank && validation.value.exist
  );

  return {
    itemUpgrades,
    isLoading,
    relativeEntries,
    baseItem,
    nextItem,
    validation,
    canUpgrade,
  };
};
