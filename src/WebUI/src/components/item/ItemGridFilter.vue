<script setup lang="ts">
import { ItemType } from '@/models/item';
import { type Buckets } from '@/models/item-search';

import { humanizeBucket } from '@/services/item-service';

const { modelValue, buckets } = defineProps<{
  modelValue: ItemType[];
  buckets: Buckets;
}>();

const emit = defineEmits<{
  'update:modelValue': [type: ItemType[]];
}>();

const itemTypeModel = computed({
  set(value: ItemType) {
    if (value === ItemType.Undefined) {
      emit('update:modelValue', []);
      return;
    }

    emit('update:modelValue', [value]);
  },

  get() {
    if (modelValue.length === 0) {
      return ItemType.Undefined;
    }

    return modelValue[0];
  },
});
</script>

<template>
  <OTabs v-model="itemTypeModel" type="fill-rounded" vertical>
    <OTabItem :value="ItemType.Undefined">
      <template #header>
        <OIcon icon="grid" size="xl" v-tooltip.bottom="'All'" />
      </template>
    </OTabItem>
    <OTabItem v-for="bucket in buckets" :value="bucket.key">
      <template #header>
        <OIcon
          :icon="humanizeBucket('type', bucket.key).icon!.name"
          size="xl"
          v-tooltip.bottom="humanizeBucket('type', bucket.key).label"
        />
      </template>
    </OTabItem>
  </OTabs>
</template>
