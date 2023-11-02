import { type Item } from '@/models/item';
import { getItemImage, getRankColor } from '@/services/item-service';

export const useItem = (item: Ref<Item>) => {
  const rankColor = computed(() => getRankColor(item.value.rank));

  const thumb = computed(() => getItemImage(item.value.baseId));

  return {
    rankColor,
    thumb,
  };
};
