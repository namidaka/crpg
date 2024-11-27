<script setup lang="ts">
import type { Setting } from '~/models/setting'

import { usePagination } from '~/composables/use-pagination'

const {
  settings,
  loading,
} = defineProps<{
  settings: Setting[]
  loading: boolean
}>()

defineEmits<{
  openSetting: [setting: Setting]
}>()

const { pageModel, perPage } = usePagination()
</script>

<template>
  <OTable
    v-model:current-page="pageModel"
    :data="settings"
    :per-page="perPage"
    :paginated="settings.length > perPage"
    hoverable
    bordered
    :loading="loading"
    :debounce-search="300"
    sort-icon="chevron-up"
    sort-icon-size="xs"
    :default-sort="['id', 'desc']"
    @click="(row) => $emit('openSetting', row as Setting)"
  >
    <OTableColumn
      v-slot="{ row: setting }: { row: Setting }"
      field="id"
      :width="40"
      label="Id"
      sortable
    >
      {{ setting.id }}
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: setting }: { row: Setting }"
      field="updatedAt"
      :width="60"
      label="Updated at"
      sortable
    >
      {{ $d(setting.updatedAt, 'short') }}
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: setting }: { row: Setting }"
      field="private"
      label="Access"
      :width="90"
      sortable
    >
      <Tag
        v-if="setting.private"
        v-tooltip="
          'TODO: add description'
        "
        label="Private"
        variant="primary"
        size="sm"
      />
      <Tag
        v-else
        variant="success"
        size="sm"
        disabled
        label="Public"
      />
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: setting }: { row: Setting }"
      field="key"
      :width="90"
      label="Key"
    >
      {{ setting.key }}
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: setting }: { row: Setting }"
      field="value"
      :width="120"
      label="Value"
    >
      {{ setting.value }}
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: setting }: { row: Setting }"
      field="description"
      :width="160"
      label="Description"
    >
      {{ setting.description }}
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: setting }: { row: Setting }"
      field="dataType"
      :width="100"
      label="DataType"
    >
      {{ setting.dataType }}
    </OTableColumn>

    <template #empty>
      <ResultNotFound />
    </template>
  </OTable>
</template>
