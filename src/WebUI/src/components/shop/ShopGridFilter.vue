<script setup lang="ts">
import type { ItemFlat } from '~/models/item'
import type { Aggregation, SortingConfig } from '~/models/item-search'

import { AggregationView } from '~/models/item-search'
import {
  getBucketValues,
  getMaxRange,
  getMinRange,
  getStepRange,
} from '~/services/item-search-service'

const { sortingConfig } = defineProps<{
  sortingConfig: SortingConfig
  aggregation: itemsjs.SearchAggregation<ItemFlat, keyof ItemFlat>
  scopeAggregation: itemsjs.SearchAggregation<ItemFlat, keyof ItemFlat>
  aggregationConfig: Aggregation
}>()
const filterModel = defineModel<string[] | number[]>('filter', { default: () => [] })
const sortingModel = defineModel<string>('sorting', { default: '' })

const sortingKeys = computed(() => Object.keys(sortingConfig))
const selfSortingIsActive = computed(() => sortingKeys.value.includes(sortingModel.value))
const isSortingAsc = computed(
  () => selfSortingIsActive.value && sortingConfig[sortingModel.value].order === 'asc',
)

const toggleSort = () => {
  const [sortAsc, sortDesc] = sortingKeys.value
  sortingModel.value = sortingModel.value === sortAsc ? sortDesc : sortAsc
}
</script>

<template>
  <THDropdown
    :shown-reset="Boolean(filterModel.length)"
    @reset="filterModel = []"
  >
    <template #label>
      <Tooltip
        :label="$t(`item.aggregations.${aggregation.name}.title`)"
        :title="$t(`item.aggregations.${aggregation.name}.title`)"
        :description="$t(`item.aggregations.${aggregation.name}.description`)"
        :delay="{ show: 300 }"
      />
    </template>

    <template #default="{ hide }">
      <template v-if="aggregationConfig.view === AggregationView.Checkbox">
        <DropdownItem
          v-for="bucket in aggregation.buckets"
          :key="(bucket.key as string)"
        >
          <ShopGridFilterCheckboxItem
            v-model="filterModel"
            :aggregation="aggregation.name"
            :bucket-value="bucket.key"
            :doc-count="bucket.doc_count"
            @update:model-value="hide"
          />
        </DropdownItem>
      </template>

      <div
        v-else-if="aggregationConfig.view === AggregationView.Range"
        class="px-8 py-3"
      >
        <SliderInput
          v-model="(filterModel as number[])"
          :min="getMinRange(getBucketValues(scopeAggregation.buckets))"
          :max="getMaxRange(getBucketValues(scopeAggregation.buckets))"
          :step="getStepRange(getBucketValues(scopeAggregation.buckets))"
        />
      </div>
    </template>

    <template
      v-if="Boolean(sortingKeys.length)"
      #labelAppend
    >
      <div
        v-tooltip="$t(isSortingAsc ? 'shop.sort.desc' : 'shop.sort.asc')"
        class="flex w-5 cursor-pointer flex-col hover:text-content-100"
        @click="toggleSort"
      >
        <OIcon
          v-if="!selfSortingIsActive || isSortingAsc"
          class="-my-1"
          icon="chevron-up"
          :size="selfSortingIsActive ? 'sm' : 'xs'"
        />
        <OIcon
          v-if="!selfSortingIsActive || !isSortingAsc"
          class="-my-1"
          icon="chevron-down"
          :size="selfSortingIsActive ? 'sm' : 'xs'"
        />
      </div>
    </template>
  </THDropdown>
</template>
