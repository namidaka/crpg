<script setup lang="ts">
import { type ItemFlat } from '@/models/item';
import { type Aggregation, AggregationView, type SortingConfig } from '@/models/item-search';
import {
  getMinRange,
  getMaxRange,
  getStepRange,
  getBucketValues,
} from '@/services/item-search-service';

const filterModel = defineModel<string[] | number[]>('filter', { default: () => [] });
const sortingModel = defineModel<string>('sorting', { default: '' });

const { sortingConfig } = defineProps<{
  sortingConfig: SortingConfig;
  aggregation: itemsjs.SearchAggregation<ItemFlat, keyof ItemFlat>;
  scopeAggregation: itemsjs.SearchAggregation<ItemFlat, keyof ItemFlat>;
  aggregationConfig: Aggregation;
}>();

const sortingKeys = computed(() => Object.keys(sortingConfig));
const selfSortingIsActive = computed(() => sortingKeys.value.includes(sortingModel.value));
const isSortingAsc = computed(
  () => selfSortingIsActive.value && sortingConfig[sortingModel.value].order === 'asc'
);

const toggleSort = () => {
  const [sortAsc, sortDesc] = sortingKeys.value;
  console.log('sortingModel.value', sortingModel.value);

  sortingModel.value = sortingModel.value === sortAsc ? sortDesc : sortAsc;
};
</script>

<template>
  <THDropdown :shownReset="Boolean(filterModel.length)" @reset="filterModel = []">
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
        <DropdownItem v-for="bucket in aggregation.buckets">
          <ShopGridFilterCheckboxItem
            v-model="filterModel"
            :aggregation="aggregation.name"
            :bucketValue="bucket.key"
            :docCount="bucket.doc_count"
            @update:modelValue="hide"
          />
        </DropdownItem>
      </template>

      <div v-else-if="aggregationConfig.view === AggregationView.Range" class="px-8 py-3">
        <SliderInput
          v-model="filterModel"
          :min="getMinRange(getBucketValues(scopeAggregation.buckets))"
          :max="getMaxRange(getBucketValues(scopeAggregation.buckets))"
          :step="getStepRange(getBucketValues(scopeAggregation.buckets))"
        />
      </div>
    </template>

    <template #labelAppend v-if="Boolean(sortingKeys.length)">
      <div
        class="flex w-5 cursor-pointer flex-col hover:text-content-100"
        v-tooltip="$t(isSortingAsc ? 'shop.sort.desc' : 'shop.sort.asc')"
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
