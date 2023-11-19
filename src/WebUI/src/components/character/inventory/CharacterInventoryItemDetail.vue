<script setup lang="ts">
import { itemSellCostPenalty } from '@root/data/constants.json';
import { type CompareItemsResult, type ItemFlat } from '@/models/item';
import { type UserItem, type UserPublic } from '@/models/user';
import {
  getAggregationsConfig,
  getVisibleAggregationsConfig,
} from '@/services/item-search-service';
import {
  computeSalePrice,
  computeBrokenItemRepairCost,
  canUpgrade,
  canAddedToClanArmory,
} from '@/services/item-service';
import { parseTimestamp } from '@/utils/date';
import { omitPredicate } from '@/utils/object';
import { useUserStore } from '@/stores/user';

const {
  item,
  userItem,
  compareResult,
  equipped = false,
} = defineProps<{
  item: ItemFlat;
  userItem: UserItem;
  compareResult?: CompareItemsResult;
  equipped?: boolean;
  lender?: UserPublic | null;
}>();

const { user, clan } = toRefs(useUserStore());

const userItemToReplaceSalePrice = computed(() => {
  const { price, graceTimeEnd } = computeSalePrice(userItem);
  return {
    price,
    graceTimeEnd:
      graceTimeEnd === null ? null : parseTimestamp(graceTimeEnd.valueOf() - new Date().valueOf()),
  };
});

const repairCost = computed(() => computeBrokenItemRepairCost(item.price));

const emit = defineEmits<{
  sell: [];
  repair: [];
  upgrade: [];
  reforge: [];
  addToClanArmory: [];
  removeFromClanArmory: [];
  returnToClanArmory: [];
}>();

const omitEmptyParam = (field: keyof ItemFlat) => {
  if (Array.isArray(item[field]) && (item[field] as string[]).length === 0) {
    return false;
  }

  if (item[field] === 0) {
    return false;
  }

  return true;
};

const aggregationsConfig = computed(() =>
  omitPredicate(
    getVisibleAggregationsConfig(getAggregationsConfig(item.type, item.weaponClass)),
    (key: keyof ItemFlat) => omitEmptyParam(key)
  )
);

const isOwnArmoryItem = computed(() => userItem.isArmoryItem && userItem.userId === user.value!.id);
const isSellable = computed(() => userItem.item.rank <= 0 && !userItem.isArmoryItem);
const isUpgradable = computed(() => canUpgrade(item.type) && !userItem.isArmoryItem);
const isCanAddedToClanArmory = computed(() => canAddedToClanArmory(item.type));
</script>

<template>
  <ItemDetail :item="userItem.item" :compareResult="compareResult">
    <template #badges-bottom-right>
      <Tag
        v-if="equipped"
        size="lg"
        icon="check"
        variant="success"
        rounded
        v-tooltip="$t('character.inventory.item.equipped')"
      />

      <Tag
        v-if="userItem.isBroken"
        rounded
        size="lg"
        icon="error"
        class="cursor-default text-status-danger opacity-80 hover:opacity-100"
        v-tooltip="$t('character.inventory.item.broken.tooltip.title')"
      />

      <CharacterInventoryItemArmoryTag v-if="lender || userItem.isArmoryItem" :lender="lender" />
    </template>

    <template #actions>
      <ConfirmActionTooltip
        v-if="isSellable"
        class="flex-auto"
        :confirmLabel="$t('action.sell')"
        :title="$t('character.inventory.item.sell.confirm')"
        @confirm="emit('sell')"
      >
        <OButton variant="secondary" expanded rounded size="lg">
          <i18n-t
            scope="global"
            keypath="character.inventory.item.sell.title"
            tag="span"
            class="flex gap-2"
          >
            <template #price>
              <Coin :value="userItemToReplaceSalePrice.price" />
            </template>
          </i18n-t>

          <VTooltip>
            <Tag
              v-if="userItemToReplaceSalePrice.graceTimeEnd !== null"
              size="sm"
              variant="success"
              :label="$n(1, 'percent', { minimumFractionDigits: 0 })"
            />
            <Tag
              v-else
              size="sm"
              variant="danger"
              :label="$n(itemSellCostPenalty, 'percent', { minimumFractionDigits: 0 })"
            />

            <template #popper>
              <i18n-t
                v-if="userItemToReplaceSalePrice.graceTimeEnd !== null"
                scope="global"
                keypath="character.inventory.item.sell.freeRefund"
                tag="div"
              >
                <template #dateTime>
                  <span class="font-bold">
                    {{ $t('dateTimeFormat.mm', { ...userItemToReplaceSalePrice.graceTimeEnd }) }}
                  </span>
                </template>
              </i18n-t>
              <i18n-t
                v-else
                scope="global"
                keypath="character.inventory.item.sell.penaltyRefund"
                tag="div"
              >
                <template #penalty>
                  <span class="font-bold text-status-danger">
                    {{ $n(itemSellCostPenalty, 'percent', { minimumFractionDigits: 0 }) }}
                  </span>
                </template>
              </i18n-t>
            </template>
          </VTooltip>
        </OButton>
      </ConfirmActionTooltip>

      <ConfirmActionTooltip v-if="userItem.isBroken" @confirm="emit('repair')">
        <VTooltip>
          <OButton iconRight="repair" variant="danger" size="lg" rounded />
          <template #popper>
            <i18n-t
              scope="global"
              keypath="character.inventory.item.repair.tooltip.title"
              tag="span"
              class="flex gap-2"
            >
              <template #price>
                <Coin :value="repairCost" />
              </template>
            </i18n-t>
          </template>
        </VTooltip>
      </ConfirmActionTooltip>

      <Modal v-if="isUpgradable" closable :autoHide="false">
        <OButton
          variant="secondary"
          rounded
          size="lg"
          iconLeft="blacksmith"
          v-tooltip="$t('character.inventory.item.upgrade.upgradesTitle')"
        />
        <template #popper>
          <div class="container pb-2 pt-12">
            <CharacterInventoryItemUpgrades
              :item="item"
              :cols="aggregationsConfig"
              @upgrade="emit('upgrade')"
              @reforge="emit('reforge')"
            />
          </div>
        </template>
      </Modal>

      <!-- CLAN ARMORY -->

      <template v-if="clan && isCanAddedToClanArmory">
        <ConfirmActionTooltip
          v-if="!userItem.isArmoryItem"
          class="flex-auto"
          :confirmLabel="$t('action.ok')"
          :title="$t('clan.armory.item.add.confirm.description')"
          @confirm="$emit('addToClanArmory')"
        >
          <OButton
            variant="secondary"
            icon-left="armory"
            rounded
            expanded
            size="lg"
            :label="$t('clan.armory.item.add.title')"
          />
        </ConfirmActionTooltip>

        <template v-else>
          <ConfirmActionTooltip
            v-if="isOwnArmoryItem"
            class="flex-auto"
            :confirmLabel="$t('action.ok')"
            :title="$t('clan.armory.item.remove.confirm.description')"
            @confirm="$emit('removeFromClanArmory')"
          >
            <OButton
              variant="warning"
              expanded
              rounded
              size="lg"
              :label="$t('clan.armory.item.remove.title')"
            />
          </ConfirmActionTooltip>

          <OButton
            v-else
            variant="secondary"
            icon-left="armory"
            expanded
            rounded
            size="lg"
            :label="$t('clan.armory.item.return.title')"
            @click="$emit('returnToClanArmory')"
          />
        </template>
      </template>
    </template>
  </ItemDetail>
</template>
