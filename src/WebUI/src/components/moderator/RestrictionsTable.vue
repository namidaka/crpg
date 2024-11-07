<script setup lang="ts">
import type { RestrictionWithActive } from '~/models/restriction'

import { usePagination } from '~/composables/use-pagination'
import { computeLeftMs, parseTimestamp } from '~/utils/date'

defineProps<{ restrictions: RestrictionWithActive[], hiddenCols?: string[] }>()

const { pageModel, perPage } = usePagination()
</script>

<template>
  <OTable
    v-model:current-page="pageModel"
    :data="restrictions"
    :per-page="perPage"
    :paginated="restrictions.length > perPage"
    hoverable
    bordered
    narrowed
    :debounce-search="300"
    sort-icon="chevron-up"
    sort-icon-size="xs"
    :default-sort="['id', 'desc']"
  >
    <OTableColumn
      v-slot="{ row: restriction }: { row: RestrictionWithActive }"
      field="id"
      :width="60"
      label="id"
      sortable
    >
      {{ restriction.id }}
    </OTableColumn>
    <OTableColumn
      v-slot="{ row: restriction }: { row: RestrictionWithActive }"
      field="active"
      :label="$t('restriction.table.column.status')"
      :width="90"
      sortable
    >
      <Tag
        v-if="restriction.active"
        v-tooltip="
          $t('dateTimeFormat.dd:hh:mm', {
            ...parseTimestamp(computeLeftMs(restriction.createdAt, restriction.duration)),
          })
        "
        :label="$t('restriction.status.active')"
        variant="success"
        size="sm"
      />
      <Tag
        v-else
        variant="info"
        size="sm"
        disabled
        :label="$t('restriction.status.inactive')"
      />
    </OTableColumn>

    <OTableColumn
      v-if="!hiddenCols?.includes('restrictedUser')"
      field="restrictedUser.name"
      :label="$t('restriction.table.column.user')"
      searchable
    >
      <template #searchable="props">
        <OInput
          v-model="props.filters[props.column.field]"
          :placeholder="$t('action.search')"
          icon="search"
          class="w-40"
          size="xs"
          clearable
        />
      </template>

      <template #default="{ row: restriction }: { row: RestrictionWithActive }">
        <RouterLink
          :to="{
            name: 'ModeratorUserIdRestrictions',
            params: { id: restriction.restrictedUser.id },
          }"
          class="inline-block hover:text-content-100"
        >
          <UserMedia
            class="max-w-48"
            :user="restriction.restrictedUser"
            hidden-clan
          />
        </RouterLink>
      </template>
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: restriction }: { row: RestrictionWithActive }"
      field="createdAt"
      :label="$t('restriction.table.column.createdAt')"
      :width="160"
      sortable
    >
      {{ $d(restriction.createdAt, 'short') }}
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: restriction }: { row: RestrictionWithActive }"
      field="duration"
      :label="$t('restriction.table.column.duration')"
      :width="160"
    >
      {{
        $t('dateTimeFormat.dd:hh:mm', {
          ...parseTimestamp(restriction.duration),
        })
      }}
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: restriction }: { row: RestrictionWithActive }"
      field="type"
      :label="$t('restriction.table.column.type')"
      :width="60"
    >
      {{ $t(`restriction.type.${restriction.type}`) }}
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: restriction }: { row: RestrictionWithActive }"
      field="reason"
      :label="$t('restriction.table.column.reason')"
    >
      <CollapsibleText :text="restriction.reason" />
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: restriction }: { row: RestrictionWithActive }"
      field="publicReason"
      :label="$t('restriction.table.column.publicReason')"
    >
      <CollapsibleText :text="restriction.publicReason" />
    </OTableColumn>

    <OTableColumn
      v-slot="{ row: restriction }: { row: RestrictionWithActive }"
      field="restrictedByUser.name"
      :label="$t('restriction.table.column.restrictedBy')"
      :width="200"
    >
      <UserMedia
        :user="restriction.restrictedByUser"
        class="max-w-48"
        hidden-clan
      />
    </OTableColumn>

    <template #empty>
      <ResultNotFound />
    </template>
  </OTable>
</template>
