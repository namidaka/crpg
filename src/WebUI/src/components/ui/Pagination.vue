<script setup lang="ts">
const {
  order = 'right',
  perPage,
  total,
  withInput = false,
} = defineProps<{
  total: number
  perPage: number
  order?: 'left' | 'right'
  withInput?: boolean
}>()

const modelValue = defineModel<number>()
</script>

<template>
  <div class="flex items-center gap-4">
    <OPagination
      v-model:current="modelValue"
      :total="total"
      :range-before="2"
      :range-after="2"
      :per-page="perPage"
      :order="order"
    >
      <!-- hidden prev/next -->
      <template #next>
        <span />
      </template>
      <template #previous>
        <span />
      </template>
    </OPagination>

    <OInput
      v-if="withInput"
      v-model="modelValue"
      size="sm"
      rounded
      class="w-20"
      clearable
    />
  </div>
</template>
