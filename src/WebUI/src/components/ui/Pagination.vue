<script setup lang="ts">
const {
  total,
  perPage,
  order = 'right',
  withInput = false,
} = defineProps<{
  total: number;
  perPage: number;
  order?: 'left' | 'right';
  withInput?: boolean;
}>();

const modelValue = defineModel<number>();
</script>

<template>
  <div class="flex items-center gap-4">
    <OPagination
      v-model:current="modelValue"
      :total="total"
      :rangeBefore="2"
      :rangeAfter="2"
      :perPage="perPage"
      :order="order"
    >
      <!-- hidden prev/next -->
      <template #next>
        <span></span>
      </template>
      <template #previous>
        <span></span>
      </template>
    </OPagination>

    <OInput v-if="withInput" v-model="modelValue" size="sm" rounded class="w-20" clearable />
  </div>
</template>
