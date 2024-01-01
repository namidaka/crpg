<script setup lang="ts">
import { create, insert, remove, search, searchVector, insertMultiple } from '@orama/orama';
import { createItemIndex } from '@/services/item-search-service/indexator';
import { documentsStore as defaultDocumentsStore } from '@orama/orama/components';
import { WeaponUsage, type ItemFlat } from '@/models/item';
import {
  getItems,
  getCompareItemsResult,
  canUpgrade,
  itemIsNewDays,
} from '@/services/item-service';
import { getSearchResult } from '@/services/item-search-service';
import { notify } from '@/services/notification-service';
import { t } from '@/services/translate-service';

import { useUserStore } from '@/stores/user';

import { useItemsFilter } from '@/composables/shop/use-filters';
import { useItemsSort } from '@/composables/shop/use-sort';
import { usePagination } from '@/composables/use-pagination';
import { useItemsCompare } from '@/composables/shop/use-compare';
import { useSearchDebounced } from '@/composables/use-search-debounce';

definePage({
  meta: {
    layout: 'default',
    noStickyHeader: true,
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const userStore = useUserStore();
const userItemsIds = computed(() => userStore.userItems.map(ui => ui.item.id));

const { state: items, execute: loadItems } = useAsyncState(() => getItems(), [], {
  immediate: false,
});

await Promise.all([loadItems(), userStore.fetchUserItems()]);

// const {
//   itemTypeModel,
//   weaponClassModel,
//   filterModel,
//   updateFilter,
//   hideOwnedItemsModel,
//   filteredByClassFlatItems,
//   aggregationsConfig,
//   aggregationsConfigVisible,
//   aggregationByType,
//   aggregationByClass,
//   scopeAggregations,
// } = useItemsFilter(items.value);
// const { searchModel } = useSearchDebounced();

// const { pageModel, perPageModel, perPageConfig } = usePagination();
// const { sortingModel, sortingConfig, getSortingConfigByField } = useItemsSort(aggregationsConfig);

// const {
//   isCompare,
//   toggleCompare,
//   compareList,
//   toggleToCompareList,
//   addAllToCompareList,
//   removeAllFromCompareList,
// } = useItemsCompare();

// const searchResult = computed(() =>
//   getSearchResult({
//     items: filteredByClassFlatItems.value,
//     userItemsIds: hideOwnedItemsModel.value ? userItemsIds.value : [],
//     aggregationConfig: aggregationsConfig.value,
//     sortingConfig: sortingConfig.value,
//     sort: sortingModel.value,
//     page: pageModel.value,
//     perPage: perPageModel.value,
//     query: searchModel.value,
//     filter: {
//       ...filterModel.value,
//       ...(isCompare.value && { modId: compareList.value }),
//     },
//   })
// );

// const compareItemsResult = computed(() =>
//   !isCompare.value
//     ? null
//     : getCompareItemsResult(searchResult.value.data.items, aggregationsConfig.value)
// );

// const buyItem = async (item: ItemFlat) => {
//   await userStore.buyItem(item.id);

//   notify(t('shop.item.buy.notify.success'));
// };

// const isUpgradableCategory = computed(() => canUpgrade(itemTypeModel.value));

// const newItemCount = computed(
//   () => searchResult.value.data.aggregations.new.buckets.find(b => b.key === '1')?.doc_count ?? 0
// );

//
//
//
//
//
//
//
//
const items2 = createItemIndex(items.value);
const store = await defaultDocumentsStore.createDocumentsStore();
const itemsDB = await create({
  schema: {
    // modId: 'string',
    name: 'string',
    type: 'string',
  },

  // components: {
  //   // override partially the default documents store
  //   documentsStore: {
  //     ...store,
  //     create(orama, mapper) {
  //       console.log(orama);
  //       console.log(mapper);

  //       return {
  //         idToInternalId: ()
  //       };
  //     },
  //   },
  // },
});

await insertMultiple(itemsDB, items.value, 100);

const results = await search(itemsDB, {
  term: 'can',
  properties: ['name'],
  // limit: 0,
  facets: {
    type: {
      // size: 3,
      // order: 'DESC',
    },
    // 'categories.secondary': {
    //   size: 2,
    //   order: 'DESC',
    // },
    // rating: {
    //   ranges: [
    //     { from: 0, to: 3 },
    //     { from: 3, to: 7 },
    //     { from: 7, to: 10 },
    //   ],
    // },
    // isFavorite: {
    //   true: true,
    //   false: true,
    // },
  },
});
console.log('query', 'can');
console.log('results', results);
</script>

<template>ddd</template>
