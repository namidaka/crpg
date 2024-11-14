<script setup lang="ts">
import type { Buckets } from '~/models/item-search'

import { ItemType } from '~/models/item'
import { humanizeBucket } from '~/services/item-service'

const { buckets, modelValue } = defineProps<{
  modelValue: ItemType[]
  buckets: Buckets
}>()

const emit = defineEmits<{
  'update:modelValue': [type: ItemType[]]
}>()

const itemTypeModel = computed({
  get() {
    if (modelValue.length === 0) {
      return ItemType.Undefined
    }

    return modelValue[0]
  },

  set(value: ItemType) {
    if (value === ItemType.Undefined) {
      emit('update:modelValue', [])
      return
    }

    emit('update:modelValue', [value])
  },
})
</script>

<template>
  <OTabs
    v-model="itemTypeModel"
    type="fill-rounded"
    vertical
  >
    <OTabItem :value="ItemType.Undefined">
      <template #header>
        <OIcon
          v-tooltip.bottom="$t('item.filter.all')"
          icon="grid"
          size="xl"
        />
      </template>
    </OTabItem>
    <OTabItem
      v-for="bucket in buckets"
      :key="(bucket.key as string)"
      :value="(bucket.key as string)"
    >
      <template #header>
        <OIcon
          v-tooltip.bottom="humanizeBucket('type', bucket.key).label"
          :icon="humanizeBucket('type', bucket.key).icon!.name"
          size="xl"
        />
      </template>
    </OTabItem>
  </OTabs>
</template>
