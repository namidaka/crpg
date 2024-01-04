<script setup lang="ts">
import { type Clan, type ClanWithMemberCount } from '@/models/clan';
import { Language } from '@/models/language';
import { getClans, getFilteredClans } from '@/services/clan-service';
import { usePagination } from '@/composables/use-pagination';
import { useSearchDebounced } from '@/composables/use-search-debounce';
import { useUserStore } from '@/stores/user';
import { useRegion } from '@/composables/use-region';

definePage({
  meta: {
    layout: 'default',
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const router = useRouter();
const route = useRoute();
const userStore = useUserStore();

const { pageModel, perPage } = usePagination();
const { searchModel } = useSearchDebounced();

// TODO: region as query, pagination - improve REST API
const { state: clans, execute: loadClans } = useAsyncState(() => getClans(), [], {
  immediate: false,
});

const { regionModel, regions } = useRegion();
const languagesModel = ref<Language[]>([]);

const filteredClans = computed(() =>
  getFilteredClans(clans.value, regionModel.value, languagesModel.value, searchModel.value)
);

const rowClass = (clan: ClanWithMemberCount<Clan>) =>
  userStore.clan?.id === clan.clan.id ? 'text-primary' : 'text-content-100';

const onClickRow = (clan: ClanWithMemberCount<Clan>) =>
  router.push({ name: 'ClansId', params: { id: clan.clan.id } });

await loadClans();
</script>

<template>
  <div class="container">
    <div class="mx-auto max-w-4xl py-8 md:py-16">
      <div class="mb-6 flex flex-wrap items-center justify-between gap-4">
        <OTabs v-model="regionModel" contentClass="hidden">
          <OTabItem v-for="region in regions" :label="$t(`region.${region}`, 0)" :value="region" />
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
        :perPage="perPage"
        :paginated="filteredClans.length > perPage"
        hoverable
        bordered
        sortIcon="chevron-up"
        sortIconSize="xs"
        :defaultSort="['memberCount', 'desc']"
        :rowClass="rowClass"
        @click="onClickRow"
      >
        <OTableColumn
          #default="{ row: clan }: { row: ClanWithMemberCount<Clan> }"
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
          #default="{ row: clan }: { row: ClanWithMemberCount<Clan> }"
          field="clan.name"
          :label="$t('clan.table.column.name')"
        >
          {{ clan.clan.name }}
          <span v-if="userStore.clan?.id === clan.clan.id" data-aq-clan-row="self-clan">
            ({{ $t('you') }})
          </span>
        </OTableColumn>

        <OTableColumn
          #default="{ row: clan }: { row: ClanWithMemberCount<Clan> }"
          field="clan.region"
          :label="$t('clan.table.column.region')"
          :width="220"
        >
          <span :class="userStore.clan?.id === clan.clan.id ? 'text-primary' : 'text-content-300'">
            {{ $t(`region.${clan.clan.region}`, 0) }}
          </span>
        </OTableColumn>

        <OTableColumn field="clan.languages" :width="220">
          <template #header>
            <div class="relative mr-2 flex items-center gap-1">
              <OIcon
                v-if="languagesModel?.length"
                class="absolute -left-5 top-1/2 -translate-y-1/2 transform cursor-pointer hover:text-status-danger"
                v-tooltip.bottom="$t('action.reset')"
                icon="close"
                size="xs"
                @click="languagesModel = []"
              />
              <VDropdown :triggers="['click']">
                <div
                  class="max-w-[90px] cursor-pointer overflow-x-hidden text-ellipsis whitespace-nowrap border-b-2 border-dashed border-border-300 pb-0.5 text-2xs hover:text-content-100 2xl:max-w-[110px]"
                >
                  Languages
                </div>

                <template #popper="{ hide }">
                  <div class="max-h-64 max-w-md overflow-y-auto">
                    <DropdownItem v-for="l in Object.keys(Language)">
                      <OCheckbox
                        v-model="languagesModel"
                        :nativeValue="l"
                        class="items-center"
                        :label="$t(`language.${l}`) + ` - ${l}`"
                        @update:modelValue="hide"
                      />
                    </DropdownItem>
                  </div>
                </template>
              </VDropdown>
            </div>
          </template>

          <template #default="{ row: clan }: { row: ClanWithMemberCount<Clan> }">
            <div class="flex items-center gap-1.5">
              <Tag
                v-for="l in clan.clan.languages"
                :label="l"
                v-tooltip="$t(`language.${l}`)"
                variant="primary"
              />
            </div>
          </template>
        </OTableColumn>

        <OTableColumn
          #default="{ row: clan }: { row: ClanWithMemberCount<Clan> }"
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
