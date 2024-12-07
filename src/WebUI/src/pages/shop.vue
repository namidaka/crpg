<script setup lang="ts">
import type { ItemFlat } from '~/models/item'

import { useItemsCompare } from '~/composables/shop/use-compare'
import { useItemsFilter } from '~/composables/shop/use-filters'
import { useItemsSort } from '~/composables/shop/use-sort'
import { usePagination } from '~/composables/use-pagination'
import { useSearchDebounced } from '~/composables/use-search-debounce'
import { WeaponUsage } from '~/models/item'
import { getSearchResult } from '~/services/item-search-service'
import {
  canUpgrade,
  getCompareItemsResult,
  getItems,
  itemIsNewDays,
} from '~/services/item-service'
import { notify } from '~/services/notification-service'
import { t } from '~/services/translate-service'
import { useUserStore } from '~/stores/user'

definePage({
  meta: {
    layout: 'default',
    noStickyHeader: true,
    roles: ['User', 'Moderator', 'Admin'],
  },
})

const userStore = useUserStore()
const { user, userItems } = toRefs(userStore)

const { execute: loadItems, state: items } = useAsyncState(() => getItems(), [], {
  immediate: false,
})

await Promise.all([loadItems(), userStore.fetchUserItems()])

const userItemsIds = computed(() => userItems.value.map(ui => ui.item.id))

const getInInventoryItems = (baseId: string) => {
  return userItems.value.filter(ui => ui.item.baseId === baseId)
}

const {
  aggregationByClass,
  aggregationByType,
  aggregationsConfig,
  aggregationsConfigVisible,
  filteredByClassFlatItems,
  filterModel,
  hideOwnedItemsModel,
  itemTypeModel,
  scopeAggregations,
  updateFilter,
  weaponClassModel,
} = useItemsFilter(items.value)
const { searchModel } = useSearchDebounced()

const { pageModel, perPageConfig, perPageModel } = usePagination()
const { getSortingConfigByField, sortingConfig, sortingModel } = useItemsSort(aggregationsConfig)

const {
  addAllToCompareList,
  compareList,
  isCompare,
  removeAllFromCompareList,
  toggleCompare,
  toggleToCompareList,
} = useItemsCompare()

const searchResult = computed(() =>
  getSearchResult({
    aggregationConfig: aggregationsConfig.value,
    filter: {
      ...filterModel.value,
      ...(isCompare.value && { modId: compareList.value }),
    },
    items: filteredByClassFlatItems.value,
    page: pageModel.value,
    perPage: perPageModel.value,
    query: searchModel.value,
    sort: sortingModel.value,
    sortingConfig: sortingConfig.value,
    userItemsIds: hideOwnedItemsModel.value ? userItemsIds.value : [],
  }),
)

const compareItemsResult = computed(() =>
  !isCompare.value
    ? null
    : getCompareItemsResult(searchResult.value.data.items, aggregationsConfig.value),
)

const buyItem = async (item: ItemFlat) => {
  await userStore.buyItem(item.id)

  notify(t('shop.item.buy.notify.success'))
}

const isUpgradableCategory = computed(() => canUpgrade(itemTypeModel.value))

const newItemCount = computed(
  () => searchResult.value.data.aggregations.new.buckets.find(b => b.key === '1')?.doc_count ?? 0,
)
</script>

<template>
  <div class="relative space-y-2 p-6">
    <div class="mb-2 flex items-center gap-6 overflow-x-auto pb-2">
      <VDropdown
        :triggers="['click']"
        placement="bottom-end"
      >
        <MoreOptionsDropdownButton
          :active="
            hideOwnedItemsModel
              || Boolean('weaponUsage' in filterModel && filterModel.weaponUsage!.length > 1)
              || Boolean('new' in filterModel && filterModel.new!.length)
          "
        />

        <template #popper="{ hide }">
          <!-- TODO: to cmp -->
          <DropdownItem>
            <Tooltip
              :title="$t('item.aggregations.new.title')"
              :description="$t('item.aggregations.new.description', { days: itemIsNewDays })"
            >
              <OCheckbox
                :native-value="1"
                :model-value="filterModel.new"
                :disabled="newItemCount === 0"
                @update:model-value="(val) => updateFilter('new', val)"
                @change="hide"
              >
                {{ $t('item.aggregations.new.title') }}
                ({{ newItemCount }})
              </OCheckbox>
            </Tooltip>
          </DropdownItem>

          <DropdownItem>
            <OCheckbox
              v-model="hideOwnedItemsModel"
              @change="hide"
            >
              {{ $t('shop.hideOwnedItems.title') }}
            </OCheckbox>
          </DropdownItem>

          <DropdownItem v-if="'weaponUsage' in filterModel">
            <Tooltip
              :title="$t('shop.nonPrimaryWeaponMode.tooltip.title')"
              :description="$t('shop.nonPrimaryWeaponMode.tooltip.desc')"
            >
              <OCheckbox
                :native-value="WeaponUsage.Secondary"
                :model-value="filterModel.weaponUsage"
                @update:model-value="(val: string) => updateFilter('weaponUsage', val)"
                @change="hide"
              >
                {{ $t('shop.nonPrimaryWeaponMode.title') }}
              </OCheckbox>
            </Tooltip>
          </DropdownItem>
        </template>
      </VDropdown>

      <div class="h-8 w-px select-none bg-border-200" />

      <ShopItemTypeSelect
        v-model:item-type="itemTypeModel"
        v-model:weapon-class="weaponClassModel"
        :item-type-buckets="aggregationByType.data.buckets"
        :weapon-class-buckets="aggregationByClass.data.buckets"
      />
    </div>

    <OTable
      v-model:current-page="pageModel"
      v-model:checked-rows="compareList"
      :data="searchResult.data.items"
      bordered
      narrowed
      hoverable
      sort-icon="chevron-up"
      sort-icon-size="xs"
      sticky-header
      :detailed="isUpgradableCategory"
      detail-key="id"
      custom-row-key="id"
      :loading="userStore.buyingItem"
    >
      <OTableColumn
        field="compare"
        :width="36"
      >
        <template #header>
          <span class="inline-flex items-center">
            <OCheckbox
              v-tooltip="
                compareList.length ? $t('shop.compare.removeAll') : $t('shop.compare.addAll')
              "
              :model-value="compareList.length >= 1"
              :native-value="true"
              @update:model-value="
                () =>
                  compareList.length
                    ? removeAllFromCompareList()
                    : addAllToCompareList(searchResult.data.items.map(item => item.modId))
              "
            />
          </span>
        </template>
        <template #default="{ row: item }: { row: ItemFlat }">
          <span class="inline-flex items-center">
            <OCheckbox
              v-tooltip="
                compareList.includes(item.modId)
                  ? $t('shop.compare.remove')
                  : $t('shop.compare.add')
              "
              :model-value="compareList.includes(item.modId)"
              :native-value="true"
              @update:model-value="() => toggleToCompareList(item.modId)"
            />
          </span>
        </template>
      </OTableColumn>

      <OTableColumn field="name">
        <template #header>
          <div class="max-w-[220px]">
            <OInput
              v-model="searchModel"
              type="text"
              :placeholder="$t('action.search')"
              icon="search"
              rounded
              expanded
              clearable
              size="sm"
              icon-right-clickable
              data-aq-search-shop-input
            />
          </div>
        </template>

        <template #default="{ row: item }: { row: ItemFlat }">
          <ShopGridItemName
            :item="item"
            show-tier
          />
        </template>
      </OTableColumn>

      <OTableColumn
        v-for="(field, idx) in (Object.keys(aggregationsConfigVisible) as Array<keyof ItemFlat>)"
        :key="idx"
        :field="field"
        :width="aggregationsConfigVisible[field]?.width ?? 140"
      >
        <template #header>
          <ShopGridFilter
            v-if="field in searchResult.data.aggregations"
            v-model:sorting="sortingModel"
            :scope-aggregation="scopeAggregations[field]"
            :aggregation="searchResult.data.aggregations[field]"
            :aggregation-config="aggregationsConfig[field]!"
            :filter="filterModel[field]!"
            :sorting-config="getSortingConfigByField(field)"
            @update:filter="val => updateFilter(field, val)"
          />
        </template>

        <template #default="{ row: item }: { row: ItemFlat }">
          <ItemParam
            :item="item"
            :field="field"
            :best-value="compareItemsResult !== null ? compareItemsResult[field] : undefined"
            :is-compare="isCompare"
          >
            <template
              v-if="field === 'upkeep'"
              #default="{ rawBuckets }"
            >
              <Coin>
                {{ $t('item.format.upkeep', { upkeep: $n(rawBuckets as number) }) }}
              </Coin>
            </template>

            <template
              v-else-if="field === 'price'"
              #default="{ rawBuckets }"
            >
              <ShopGridItemBuyBtn
                :price="(rawBuckets as number)"
                :upkeep="item.upkeep"
                :in-inventory-items="getInInventoryItems(item.baseId)"
                :not-enough-gold="user!.gold < item.price"
                @buy="buyItem(item)"
              />
            </template>
          </ItemParam>
        </template>
      </OTableColumn>

      <template #detail="{ row: item }: { row: ItemFlat }">
        <ShopGridUpgradesTable
          :item="item"
          :cols="aggregationsConfigVisible"
        />
      </template>

      <template #empty>
        <ResultNotFound />
      </template>

      <template #footer>
        <div class="space-y-4 bg-base-100 py-4 pr-2 backdrop-blur-sm">
          <div class="grid h-14 grid-cols-3 items-center gap-6">
            <Pagination
              v-model="pageModel"
              :total="searchResult.pagination.total"
              :per-page="searchResult.pagination.per_page"
              order="left"
              with-input
            />

            <div class="flex justify-center">
              <OButton
                v-if="compareList.length >= 2"
                variant="primary"
                size="lg"
                outlined
                :icon-right="isCompare ? 'close' : ''"
                data-aq-shop-handler="toggle-compare"
                :label="$t('shop.compare.title')"
                @click="toggleCompare"
              />
            </div>

            <div class="flex items-center justify-end gap-4">
              <div class="text-content-400">
                {{ $t('shop.pagination.perPage') }}
              </div>
              <OTabs
                v-model="perPageModel"
                size="xl"
                type="bordered-rounded"
                content-class="hidden"
              >
                <OTabItem
                  v-for="pp in perPageConfig"
                  :key="pp"
                  :label="String(pp)"
                  :value="pp"
                />
              </OTabs>
            </div>
          </div>
        </div>
      </template>
    </OTable>
  </div>
</template>
