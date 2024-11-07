<script setup lang="ts">
import type { Clan, ClanWithMemberCount } from '~/models/clan'

import { useLanguages } from '~/composables/use-language'
import { usePagination } from '~/composables/use-pagination'
import { useRegion } from '~/composables/use-region'
import { useSearchDebounced } from '~/composables/use-search-debounce'
import { getClans, getFilteredClans } from '~/services/clan-service'
import { useUserStore } from '~/stores/user'

definePage({
  meta: {
    layout: 'default',
    roles: ['User', 'Moderator', 'Admin'],
  },
})

const router = useRouter()
const userStore = useUserStore()

const { pageModel, perPage } = usePagination()
const { searchModel } = useSearchDebounced()

// TODO: region as query, pagination - improve REST API
const { execute: loadClans, state: clans } = useAsyncState(() => getClans(), [], {
  immediate: false,
})

const { regionModel, regions } = useRegion()
const { languages, languagesModel } = useLanguages()
const aggregatedLanguages = computed(() =>
  languages.filter(l => clans.value.some(c => c.clan.languages.includes(l))),
)
watch(regionModel, () => {
  languagesModel.value = []
})

const filteredClans = computed(() =>
  getFilteredClans(clans.value, regionModel.value, languagesModel.value, searchModel.value),
)

const rowClass = (clan: ClanWithMemberCount<Clan>) =>
  userStore.clan?.id === clan.clan.id ? 'text-primary' : 'text-content-100'

const onClickRow = (clan: ClanWithMemberCount<Clan>) =>
  router.push({ name: 'ClansId', params: { id: clan.clan.id } })

await loadClans()
</script>

<template>
  <div class="container">
    <div class="mx-auto max-w-4xl py-8 md:py-16">
      <div class="mb-6 flex flex-wrap items-center justify-between gap-4">
        <OTabs
          v-model="regionModel"
          content-class="hidden"
        >
          <OTabItem
            v-for="region in regions"
            :key="region"
            :label="$t(`region.${region}`, 0)"
            :value="region"
          />
        </OTabs>

        <div class="flex items-center gap-2">
          <div class="w-44">
            <OInput
              v-model="searchModel"
              type="text"
              expanded
              clearable
              :placeholder="$t('action.search')"
              icon="search"
              rounded
              size="sm"
              data-aq-search-clan-input
            />
          </div>

          <OButton
            v-if="userStore.clan"
            v-tooltip.bottom="$t('clan.action.goToMyClan')"
            data-aq-my-clan-button
            tag="router-link"
            rounded
            icon-left="member"
            size="sm"
            :to="{ name: 'ClansId', params: { id: userStore.clan.id } }"
            variant="secondary"
            data-aq-to-clan-button
          />
          <OButton
            v-else
            v-tooltip.bottom="$t('clan.action.create')"
            tag="router-link"
            rounded
            icon-left="add"
            :to="{ name: 'ClansCreate' }"
            variant="secondary"
            size="sm"
            data-aq-create-clan-button
          />
        </div>
      </div>

      <OTable
        v-model:current-page="pageModel"
        :data="filteredClans"
        :per-page="perPage"
        :paginated="filteredClans.length > perPage"
        hoverable
        bordered
        sort-icon="chevron-up"
        sort-icon-size="xs"
        :default-sort="['memberCount', 'desc']"
        :row-class="(row) => rowClass(row as ClanWithMemberCount<Clan>)"
        @click="(row) => onClickRow(row as ClanWithMemberCount<Clan>)"
      >
        <OTableColumn
          v-slot="{ row: clan }: { row: ClanWithMemberCount<Clan> }"
          field="clan.tag"
          :label="$t('clan.table.column.tag')"
          :width="120"
        >
          <div class="flex items-center gap-2">
            <ClanTagIcon :color="clan.clan.primaryColor" />
            {{ clan.clan.tag }}
          </div>
        </OTableColumn>

        <OTableColumn
          v-slot="{ row: clan }: { row: ClanWithMemberCount<Clan> }"
          field="clan.name"
          :label="$t('clan.table.column.name')"
        >
          {{ clan.clan.name }}
          <span
            v-if="userStore.clan?.id === clan.clan.id"
            data-aq-clan-row="self-clan"
          >
            ({{ $t('you') }})
          </span>
        </OTableColumn>

        <OTableColumn
          field="clan.languages"
          :width="220"
        >
          <template #header>
            <THDropdown
              :label="$t('clan.table.column.languages')"
              :shown-reset="Boolean(languagesModel.length)"
              @reset="languagesModel = []"
            >
              <DropdownItem
                v-for="l in aggregatedLanguages"
                :key="l"
              >
                <OCheckbox
                  v-model="languagesModel"
                  :native-value="l"
                  class="items-center"
                  :label="`${$t(`language.${l}`)} - ${l}`"
                />
              </DropdownItem>
            </THDropdown>
          </template>

          <template #default="{ row: clan }: { row: ClanWithMemberCount<Clan> }">
            <div class="flex items-center gap-1.5">
              <Tag
                v-for="l in clan.clan.languages"
                :key="l"
                v-tooltip="$t(`language.${l}`)"
                :label="l"
                variant="primary"
              />
            </div>
          </template>
        </OTableColumn>

        <OTableColumn
          v-slot="{ row: clan }: { row: ClanWithMemberCount<Clan> }"
          field="memberCount"
          :label="$t('clan.table.column.members')"
          :width="40"
          position="right"
          numeric
          sortable
        >
          {{ clan.memberCount }}
        </OTableColumn>

        <template #empty>
          <ResultNotFound />
        </template>
      </OTable>
    </div>
  </div>
</template>
