<script setup lang="ts">
import { type SortingConfig } from '@/models/item-search';

const { config } = defineProps<{
  config: SortingConfig;
}>();

const modelValue = defineModel<string>();
</script>

<template>
  <VDropdown :triggers="['click']" placement="bottom-end">
    <OButton
      variant="secondary"
      size="sm"
      type="button"
      expanded
      icon-right="chevron-down"
      :label="$t(`item.sort.${modelValue}`)"
      v-tooltip="$t('action.sort')"
    />

    <template #popper="{ hide }">
      <DropdownItem
        v-for="sort in Object.keys(config)"
        :checked="sort === modelValue"
        @click="
          () => {
            modelValue = sort;
            hide();
          }
        "
      >
        {{ $t(`item.sort.${sort}`) }}
      </DropdownItem>
    </template>
  </VDropdown>
</template>
