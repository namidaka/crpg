<script setup lang="ts">
import { useStorage } from '@vueuse/core';
import { ItemType } from '@/models/item';
import { type AggregationConfig, AggregationView, type SortingConfig } from '@/models/item-search';
import { createItemIndex } from '@/services/item-search-service/indexator';
import { getSearchResult } from '@/services/item-search-service';
import { getClanArmoryItemBorrower } from '@/services/clan-service';
import { notify } from '@/services/notification-service';
import { t } from '@/services/translate-service';
import { useClan } from '@/composables/clan/use-clan';
import { useClanArmory } from '@/composables/clan/use-clan-armory';
import { useItemDetail } from '@/composables/character/use-item-detail';
import { usePagination } from '@/composables/use-pagination';
import { useClanMembers } from '@/composables/clan/use-clan-members';
import { useUserStore } from '@/stores/user';

definePage({
  props: true,
  meta: {
    layout: 'default',
    middleware: 'canUseClanArmory',
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const props = defineProps<{
  id: string;
}>();

const userStore = useUserStore();

const { clanId, loadClan } = useClan(props.id);
const { clanArmory, loadClanArmory, isLoadingClanArmory, borrowItem, removeItem } = useClanArmory(
  clanId.value
);
const { clanMembers, loadClanMembers } = useClanMembers(clanId.value);

const onBorrowFromClanArmory = async (userItemId: number) => {
  await borrowItem(userItemId);
  await Promise.all([userStore.fetchUser(), userStore.fetchUserItems(), loadClanArmory()]);
  notify(t('clan.armory.item.borrow.notify.success'));
};

const onRemoveFromClanArmory = async (userItemId: number) => {
  await removeItem(userItemId);
  await Promise.all([userStore.fetchUser(), userStore.fetchUserItems(), loadClanArmory()]);
  notify(t('clan.armory.item.remove.notify.success'));
};

const sortingConfig: SortingConfig = {
  rank_desc: {
    field: 'rank',
    order: 'desc',
  },
  type_asc: {
    field: 'type',
    order: 'asc',
  },
};
const aggregationConfig = {
  type: {
    title: 'type',
    description: '',
    sort: 'term',
    size: 1000,
    conjunction: false,
    view: AggregationView.Radio,
    chosen_filters_on_top: false,
  },
} as AggregationConfig;

const sortingModel = ref<string>('rank_desc');
const filterByTypeModel = ref<ItemType[]>([]);
const filterByNameModel = ref<string>('');
const { pageModel } = usePagination();
const perPage = 16;
const hideOwnedItemsModel = useStorage<boolean>('clan-armory-hide-owned-items', true);
const hideBorrowedItemsModel = useStorage<boolean>('clan-armory-hide-borrowed-items', true);

const flatItems = computed(() =>
  createItemIndex(
    clanArmory.value
      .filter(
        item =>
          (hideOwnedItemsModel.value ? item.userItem.userId !== userStore.user!.id : true) &&
          (hideBorrowedItemsModel.value ? item.borrowedItem === null : true)
      )
      .map(ca => ca.userItem.item)
  )
);

const searchResult = computed(() =>
  getSearchResult({
    items: flatItems.value,
    userItemsIds: [],
    aggregationConfig: aggregationConfig,
    sortingConfig: sortingConfig,
    sort: sortingModel.value,
    page: pageModel.value,
    perPage: perPage,
    query: filterByNameModel.value,
    filter: {
      type: filterByTypeModel.value,
    },
  })
);

const filteredClanArmoryItems = computed(() => {
  const foundedItemIds = searchResult.value.data.items.map(item => item.id);
  return clanArmory.value
    .filter(item => foundedItemIds.includes(item.userItem.item.id))
    .sort((a, b) => {
      if (sortingModel.value === 'type_asc') {
        const itemTypes = Object.values(ItemType);
        return itemTypes.indexOf(a.userItem.item.type) - itemTypes.indexOf(b.userItem.item.type);
      }
      return (
        foundedItemIds.indexOf(a.userItem.item.id) - foundedItemIds.indexOf(b.userItem.item.id)
      );
    });
});

const { toggleItemDetail, closeItemDetail } = useItemDetail();

await Promise.all([loadClan(0, { id: clanId.value }), loadClanArmory(), loadClanMembers()]);
</script>

<template>
  <div class="p-6">
    <OButton
      v-tooltip.bottom="$t('nav.back')"
      tag="router-link"
      :to="{ name: 'ClansId', params: { id: clanId } }"
      variant="secondary"
      size="xl"
      outlined
      rounded
      icon-left="arrow-left"
      data-aq-link="back-to-clan"
    />

    <div class="mx-auto max-w-2xl py-6">
      <Heading class="mb-14" :title="$t('clan.armory.title')" />

      <div v-if="clanArmory.length !== 0" class="itemGrid relative grid h-full gap-x-3 gap-y-4">
        <div style="grid-area: filter" class="space-y-2">
          <VDropdown :triggers="['click']" placement="bottom-end">
            <MoreOptionsDropdownButton :active="hideOwnedItemsModel || hideBorrowedItemsModel" />

            <template #popper="{ hide }">
              <DropdownItem>
                <OCheckbox v-model="hideOwnedItemsModel" :nativeValue="true" @change="hide">
                  {{ $t('clan.armory.filter.hideOwned') }}
                </OCheckbox>
              </DropdownItem>

              <DropdownItem>
                <OCheckbox v-model="hideBorrowedItemsModel" :nativeValue="true" @change="hide">
                  {{ $t('clan.armory.filter.hideBorrowed') }}
                </OCheckbox>
              </DropdownItem>
            </template>
          </VDropdown>

          <ItemGridFilter
            v-if="'type' in searchResult.data.aggregations"
            v-model="filterByTypeModel"
            :buckets="searchResult.data.aggregations.type.buckets"
          />
        </div>

        <div class="grid grid-cols-3 gap-4 2xl:grid-cols-4" style="grid-area: sort">
          <div class="col-span-2 2xl:col-span-3">
            <OInput
              v-model="filterByNameModel"
              type="text"
              expanded
              clearable
              :placeholder="$t('action.search')"
              icon="search"
              rounded
              size="sm"
            />
          </div>

          <ItemGridSearch class="col-span-1" v-model="sortingModel" :config="sortingConfig" />
        </div>

        <div style="grid-area: items">
          <OLoading v-if="isLoadingClanArmory" active iconSize="xl" />

          <div v-else class="relative grid grid-cols-3 gap-2 2xl:grid-cols-4">
            <ClanArmoryItemCard
              v-for="clanArmoryItem in filteredClanArmoryItems"
              :clanArmoryItem="clanArmoryItem"
              :lender="clanMembers.find(cm => cm.user.id === clanArmoryItem.userItem.userId)!.user"
              :borrower="getClanArmoryItemBorrower(clanArmoryItem, clanMembers)"
              @click="
                e =>
                  toggleItemDetail(e.target as HTMLElement, {
                    id: clanArmoryItem.userItem.item.id,
                    userItemId: clanArmoryItem.userItem.id,
                  })
              "
            />
          </div>
        </div>

        <Pagination
          v-if="perPage < searchResult.pagination.total"
          style="grid-area: footer"
          class="justify-center"
          v-model="pageModel"
          :total="searchResult.pagination.total"
          :perPage="perPage"
        />
      </div>

      <ResultNotFound
        v-else
        class="rounded-xl border border-dashed border-border-300"
        :message="$t('clan.armory.empty')"
      />
    </div>

    <ItemDetailGroup>
      <template #default="di">
        <ClanArmoryItemDetail
          :clanArmoryItem="clanArmory.find(ca => ca.userItem.id === di.userItemId)!"
          :lender="
            clanMembers.find(
              cm =>
                cm.user.id === clanArmory.find(ca => ca.userItem.item.id === di.id)!.userItem.userId
            )!.user
          "
          :borrower="
            getClanArmoryItemBorrower(
              clanArmory.find(ca => ca.userItem.item.id === di.id)!,
              clanMembers
            )
          "
          @borrow="
            id => {
              closeItemDetail(di.id);
              onBorrowFromClanArmory(id);
            }
          "
          @remove="
            id => {
              closeItemDetail(di.id);
              onRemoveFromClanArmory(id);
            }
          "
        />
      </template>
    </ItemDetailGroup>
  </div>
</template>

<style lang="css">
.itemGrid {
  grid-template-areas:
    '...... sort'
    'filter items'
    'filter footer';
  grid-template-columns: auto 1fr;
  grid-template-rows: auto 1fr auto;
}
</style>
