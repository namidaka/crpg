<script setup lang="ts">
import { useStorage } from '@vueuse/core';
import { type UserItemsBySlot, UserItem } from '@/models/user';
import type { EquippedItemId } from '@/models/character';
import { ItemType } from '@/models/item';
import { AggregationConfig, AggregationView, SortingConfig } from '@/models/item-search';
import { extractItemFromUserItem } from '@/services/users-service';
import {
  updateCharacterItems,
  checkUpkeepIsHigh,
  validateItemNotMeetRequirement,
} from '@/services/characters-service';
import { useUserStore } from '@/stores/user';
import {
  sellUserItem,
  repairUserItem,
  upgradeUserItem,
  reforgeUserItem,
} from '@/services/users-service';
import {
  groupItemsByTypeAndWeaponClass,
  getCompareItemsResult,
  getLinkedSlots,
} from '@/services/item-service';
import { createItemIndex } from '@/services/item-search-service/indexator';
import { getSearchResult, getAggregationsConfig } from '@/services/item-search-service';
import { notify } from '@/services/notification-service';
import { t } from '@/services/translate-service';
import {
  returnItemToClanArmory,
  removeItemFromClanArmory,
  addItemToClanArmory,
  getClanArmoryItemLender,
} from '@/services/clan-service';
import { scrollToTop } from '@/utils/scroll';
import { useItemDetail } from '@/composables/character/use-item-detail';
import { useStickySidebar } from '@/composables/use-sticky-sidebar';
import { useInventoryDnD } from '@/composables/character/use-inventory-dnd';
import { useInventoryQuickEquip } from '@/composables/character/use-inventory-quick-equip';
import {
  characterKey,
  characterCharacteristicsKey,
  characterHealthPointsKey,
  characterItemsKey,
  characterItemsStatsKey,
  equippedItemsBySlotKey,
} from '@/symbols/character';
import { mainHeaderHeightKey } from '@/symbols/common';
import { useClanMembers } from '@/composables/clan/use-clan-members';

definePage({
  props: true,
  meta: {
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const userStore = useUserStore();
const { user, clan, userItems } = toRefs(userStore);

const character = injectStrict(characterKey);
const { characterCharacteristics } = injectStrict(characterCharacteristicsKey);
const healthPoints = injectStrict(characterHealthPointsKey);
const { characterItems, loadCharacterItems } = injectStrict(characterItemsKey);
const itemsStats = injectStrict(characterItemsStatsKey);
const mainHeaderHeight = injectStrict(mainHeaderHeightKey);

const upkeepIsHigh = computed(() =>
  checkUpkeepIsHigh(user.value!.gold, itemsStats.value.averageRepairCostByHour)
);
const equippedItemsIds = computed(() => characterItems.value.map(ei => ei.userItem.id));

const onChangeEquippedItems = async (items: EquippedItemId[]) => {
  await updateCharacterItems(character.value.id, items);
  await loadCharacterItems(0, { id: character.value.id });
};

const onSellUserItem = async (itemId: number) => {
  // unEquip linked slots
  const characterItem = characterItems.value.find(ci => ci.userItem.id === itemId);
  if (characterItem !== undefined) {
    await updateCharacterItems(character.value.id, [
      ...getLinkedSlots(characterItem.slot, equippedItemsBySlot.value).map(ls => ({
        userItemId: null,
        slot: ls,
      })),
    ]);
  }
  // if the item sold is the last item in the active category,
  // you must reset the filter because that category is no longer in inventory
  if (filteredUserItems.value.length === 1) {
    filterByTypeModel.value = [];
  }
  await sellUserItem(itemId);
  await Promise.all([
    userStore.fetchUser(),
    userStore.fetchUserItems(),
    loadCharacterItems(0, { id: character.value.id }),
  ]);
  notify(t('character.inventory.item.sell.notify.success'));
};

const onRepairUserItem = async (itemId: number) => {
  await repairUserItem(itemId);
  await Promise.all([userStore.fetchUser(), userStore.fetchUserItems()]);
  notify(t('character.inventory.item.repair.notify.success'));
};

const onUpgradeUserItem = async (itemId: number) => {
  await upgradeUserItem(itemId);
  await Promise.all([
    userStore.fetchUser(),
    userStore.fetchUserItems(),
    loadCharacterItems(0, { id: character.value.id }),
  ]);
  notify(t('character.inventory.item.upgrade.notify.success'));
};

const onReforgeUserItem = async (itemId: number) => {
  await reforgeUserItem(itemId);
  await Promise.all([
    userStore.fetchUser(),
    userStore.fetchUserItems(),
    loadCharacterItems(0, { id: character.value.id }),
  ]);
  notify(t('character.inventory.item.reforge.notify.success'));
};

const onAddItemToClanArmory = async (userItemId: number) => {
  if (filteredUserItems.value.length === 1) {
    filterByTypeModel.value = [];
  }
  await addItemToClanArmory(clan.value!.id, userItemId);
  await Promise.all([
    userStore.fetchUser(),
    userStore.fetchUserItems(),
    loadCharacterItems(0, { id: character.value.id }),
  ]);
  notify(t('clan.armory.item.add.notify.success'));
};

const onReturnToClanArmory = async (userItemId: number) => {
  await returnItemToClanArmory(clan.value!.id, userItemId);
  if (filteredUserItems.value.length === 1) {
    filterByTypeModel.value = [];
  }
  await Promise.all([
    userStore.fetchUser(),
    userStore.fetchUserItems(),
    loadCharacterItems(0, { id: character.value.id }),
  ]);
  notify(t('clan.armory.item.return.notify.success'));
};

const onRemoveFromClanArmory = async (userItemId: number) => {
  await removeItemFromClanArmory(clan.value!.id, userItemId);
  await Promise.all([
    userStore.fetchUser(),
    userStore.fetchUserItems(),
    loadCharacterItems(0, { id: character.value.id }),
  ]);
  notify(t('clan.armory.item.remove.notify.success'));
};

const onClickInventoryItem = async (e: PointerEvent, userItem: UserItem) => {
  if (e.ctrlKey) {
    if (!equippedItemsIds.value.includes(userItem.id)) {
      await onQuickEquip(userItem);
    }
  } else {
    toggleItemDetail(e.target as HTMLElement, {
      id: userItem.item.id,
      userItemId: userItem.id,
    });
  }
};

const hideInArmoryItemsModel = useStorage<boolean>('character-inventory-in-armory-items', true);
const hasArmoryItems = computed(() => userItems.value.some(ui => ui.isArmoryItem));

const flatItems = computed(() =>
  createItemIndex(
    extractItemFromUserItem(
      userItems.value.filter(item =>
        hideInArmoryItemsModel.value && item.isArmoryItem ? item.userId !== user.value!.id : true
      )
    )
  )
);

const sortingConfig: SortingConfig = {
  rank_desc: {
    field: 'rank',
    order: 'desc',
  },
  type_asc: {
    field: 'type',
    order: 'asc',
  },
  price_asc: {
    field: 'price',
    order: 'asc',
  },
  price_desc: {
    field: 'price',
    order: 'desc',
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

const filterByTypeModel = ref<ItemType[]>([]);
const filterByNameModel = ref<string>('');
const sortingModel = useStorage<string>('character-inventory-sorting', 'rank_desc');

const searchResult = computed(() =>
  getSearchResult({
    items: flatItems.value,
    userItemsIds: [],
    aggregationConfig: aggregationConfig,
    sortingConfig: sortingConfig,
    sort: sortingModel.value,
    page: 1,
    perPage: 1000,
    query: filterByNameModel.value,
    filter: {
      type: filterByTypeModel.value,
    },
  })
);

const filteredUserItems = computed(() => {
  const foundedItemIds = searchResult.value.data.items.map(item => item.id);
  return userItems.value
    .filter(
      item =>
        foundedItemIds.includes(item.item.id) &&
        (hideInArmoryItemsModel.value && item.isArmoryItem ? item.userId !== user.value!.id : true)
    )
    .sort((a, b) => {
      if (sortingModel.value === 'type_asc') {
        const itemTypes = Object.values(ItemType);
        return itemTypes.indexOf(a.item.type) - itemTypes.indexOf(b.item.type);
      }
      return foundedItemIds.indexOf(a.item.id) - foundedItemIds.indexOf(b.item.id);
    });
});

const totalItemsCost = computed(() =>
  filteredUserItems.value.reduce((out, item) => out + item.item.price, 0)
);

const equippedItemsBySlot = computed(() =>
  characterItems.value.reduce((out, ei) => {
    out[ei.slot] = ei.userItem as UserItem;
    return out;
  }, {} as UserItemsBySlot)
);
provide(equippedItemsBySlotKey, equippedItemsBySlot);

const { onDragStart, onDragEnd } = useInventoryDnD(equippedItemsBySlot);

const { onQuickEquip } = useInventoryQuickEquip(equippedItemsBySlot);

const { openedItems, toggleItemDetail, closeItemDetail, getUniqueId } = useItemDetail();

const compareItemsResult = computed(() => {
  // find the open items TODO: spec
  return groupItemsByTypeAndWeaponClass(
    createItemIndex(
      extractItemFromUserItem(
        userItems.value.filter(ui =>
          openedItems.value.some(oi => oi.uniqueId === getUniqueId(ui.item.id, ui.id))
        )
      )
    )
  )
    .filter(group => group.items.length >= 2) // there is no point in comparing 1 item;
    .map(group => ({
      type: group.type,
      weaponClass: group.weaponClass,
      compareResult: getCompareItemsResult(
        group.items,
        getAggregationsConfig(group.type, group.weaponClass)
      ),
    }));
});

const aside = ref<HTMLDivElement | null>(null);
const { top: stickySidebarTop } = useStickySidebar(aside, mainHeaderHeight.value + 16, 16);

const promises: Promise<any>[] = [userStore.fetchUserItems()];

const { clanMembers, loadClanMembers } = useClanMembers();

if (clan.value) {
  promises.push(loadClanMembers(0, { id: clan.value.id }));
}

await Promise.all(promises);
</script>

<template>
  <div class="relative grid grid-cols-12 gap-5">
    <div class="col-span-5">
      <template v-if="userItems.length !== 0">
        <div class="inventoryGrid relative grid h-full gap-x-3 gap-y-4">
          <div
            style="grid-area: filter"
            ref="aside"
            class="sticky space-y-2"
            :style="{ top: `${stickySidebarTop}px` }"
          >
            <OButton
              v-if="hasArmoryItems"
              :variant="hideInArmoryItemsModel ? 'secondary' : 'primary'"
              outlined
              size="xl"
              rounded
              icon-left="armory"
              v-tooltip.bottom="
                hideInArmoryItemsModel
                  ? $t('character.inventory.filter.showInArmory')
                  : $t('character.inventory.filter.hideInArmory')
              "
              @click="
                () => {
                  hideInArmoryItemsModel = !hideInArmoryItemsModel;
                  filterByTypeModel = [];
                }
              "
            />

            <ItemGridFilter
              v-if="'type' in searchResult.data.aggregations"
              v-model="filterByTypeModel"
              :buckets="searchResult.data.aggregations.type.buckets"
              @click="scrollToTop"
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

          <div class="relative h-full" style="grid-area: items">
            <div class="grid grid-cols-3 gap-2 2xl:grid-cols-4">
              <CharacterInventoryItemCard
                v-for="userItem in filteredUserItems"
                class="cursor-grab"
                :key="userItem.id"
                :userItem="userItem"
                :equipped="equippedItemsIds.includes(userItem.id)"
                :notMeetRequirement="
                  validateItemNotMeetRequirement(userItem.item, characterCharacteristics)
                "
                :lender="getClanArmoryItemLender(userItem, clanMembers)"
                draggable="true"
                @dragstart="onDragStart(userItem)"
                @dragend="onDragEnd"
                @click="e => onClickInventoryItem(e, userItem)"
              />
            </div>
          </div>

          <div
            class="sticky bottom-4 left-0 z-10 flex w-full justify-center rounded-lg bg-base-200 bg-opacity-60 p-4 backdrop-blur-lg"
            style="grid-area: footer"
          >
            <i18n-t
              scope="global"
              keypath="character.inventory.total.tpl"
              tag="div"
              :plural="filteredUserItems.length"
            >
              <template #count>
                <i18n-t
                  scope="global"
                  keypath="character.inventory.total.count"
                  tag="span"
                  :plural="filteredUserItems.length"
                >
                  <template #count>
                    <span class="font-bold text-content-100">
                      {{ filteredUserItems.length }}
                    </span>
                  </template>
                </i18n-t>
              </template>

              <template #sum>
                <i18n-t scope="global" tag="span" keypath="character.inventory.total.sum">
                  <template #sum>
                    <Coin :value="totalItemsCost" />
                  </template>
                </i18n-t>
              </template>
            </i18n-t>
          </div>
        </div>
      </template>

      <ResultNotFound
        v-else
        class="rounded-xl border border-dashed border-border-300"
        :message="$t('character.inventory.empty')"
      />
    </div>

    <div class="sticky left-0 col-span-5 self-start" :style="{ top: `${mainHeaderHeight + 16}px` }">
      <CharacterInventoryDoll @change="onChangeEquippedItems" />
      <div
        class="flex mt-3 w-full justify-center rounded-lg bg-base-200 p-4 backdrop-blur-lg"
        style="grid-area: footer"
      >
        <KbdCombination :keys="['CTRL', 'LMB']" :label="$t('character.doll.equipShortcutHint')" />
      </div>
    </div>


    <div
      class="sticky col-span-2 grid grid-cols-1 items-start gap-2 self-start rounded-lg border border-border-200 py-2 text-2xs"
      :style="{ top: `${mainHeaderHeight + 16}px` }"
    >
      <SimpleTableRow
        :label="$t('character.stats.price.title')"
        :tooltip="{
          title: $t('character.stats.price.title'),
          description: $t('character.stats.price.desc'),
        }"
      >
        <div class="inline-flex gap-1.5 align-middle">
          <SvgSpriteImg name="coin" viewBox="0 0 18 18" class="w-4" />
          <span class="text-xs font-bold text-primary">{{ $n(itemsStats.price) }}</span>
        </div>
      </SimpleTableRow>

      <SimpleTableRow
        :label="$t('character.stats.avgRepairCost.title')"
        :tooltip="{
          title: $t('character.stats.avgRepairCost.title'),
          description: $t('character.stats.avgRepairCost.desc'),
        }"
      >
        <div class="inline-flex gap-1.5 align-middle">
          <SvgSpriteImg name="coin" viewBox="0 0 18 18" class="w-4" />
          <ClosableTooltip :disabled="!upkeepIsHigh" shown placement="top">
            <span
              class="text-xs font-bold"
              :class="[upkeepIsHigh ? 'text-status-danger' : 'text-primary']"
            >
              {{ $n(itemsStats.averageRepairCostByHour) }} / {{ $t('dateTime.hours.short') }}
            </span>
            <template #popper>
              <div class="prose prose-invert">
                <h4 class="text-status-warning">
                  {{ $t('character.highUpkeepWarning.title') }}
                </h4>
                <div v-html="$t('character.highUpkeepWarning.desc')"></div>
              </div>
            </template>
          </ClosableTooltip>
        </div>
      </SimpleTableRow>

      <CharacterStats
        :characteristics="characterCharacteristics"
        :weight="itemsStats.weight"
        :longestWeaponLength="itemsStats.longestWeaponLength"
        :healthPoints="healthPoints"
      />
    </div>

    <ItemDetailGroup>
      <template #default="di">
        <CharacterInventoryItemDetail
          :compareResult="
            compareItemsResult.find(cr => cr.type === flatItems.find(fi => fi.id === di.id)!.type)
              ?.compareResult
          "
          :userItem="userItems.find(ui => ui.id === di.userItemId)!"
          :equipped="equippedItemsIds.includes(di.userItemId)"
          :lender="
            getClanArmoryItemLender(userItems.find(ui => ui.id === di.userItemId)!, clanMembers)
          "
          @sell="
            () => {
              closeItemDetail(di);
              onSellUserItem(di.userItemId);
            }
          "
          @repair="
            () => {
              closeItemDetail(di);
              onRepairUserItem(di.userItemId);
            }
          "
          @upgrade="
            () => {
              closeItemDetail(di);
              onUpgradeUserItem(di.userItemId);
            }
          "
          @reforge="
            () => {
              closeItemDetail(di);
              onReforgeUserItem(di.userItemId);
            }
          "
          @returnToClanArmory="
            () => {
              closeItemDetail(di);
              onReturnToClanArmory(di.userItemId);
            }
          "
          @removeFromClanArmory="
            () => {
              closeItemDetail(di);
              onRemoveFromClanArmory(di.userItemId);
            }
          "
          @addToClanArmory="
            () => {
              closeItemDetail(di);
              onAddItemToClanArmory(di.userItemId);
            }
          "
        />
      </template>
    </ItemDetailGroup>
  </div>
</template>

<style lang="css">
.inventoryGrid {
  grid-template-areas:
    '...... sort'
    'filter items'
    'filter footer';
  grid-template-columns: auto 1fr;
  grid-template-rows: auto 1fr auto;
}
</style>
