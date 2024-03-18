<script setup lang="ts">
import { itemSellCostPenalty } from '@root/data/constants.json';
import { type CompareItemsResult } from '@/models/item';
import { type UserItem, type UserPublic } from '@/models/user';

import {
  computeSalePrice,
  computeBrokenItemRepairCost,
  canUpgrade,
  canAddedToClanArmory,
} from '@/services/item-service';
import { parseTimestamp } from '@/utils/date';
import { useUserStore } from '@/stores/user';

const {
  userItem,
  compareResult,
  equipped = false,
} = defineProps<{
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

const repairCost = computed(() => computeBrokenItemRepairCost(userItem.item.price));

const emit = defineEmits<{
  sell: [];
  repair: [];
  upgrade: [];
  reforge: [];
  addToClanArmory: [];
  removeFromClanArmory: [];
  returnToClanArmory: [];
}>();

const isOwnArmoryItem = computed(() => userItem.isArmoryItem && userItem.userId === user.value!.id);
const isSellable = computed(
  () => userItem.item.rank <= 0 && !userItem.isArmoryItem && !userItem.isPersonal
);
const isUpgradable = computed(() => canUpgrade(userItem.item.type) && !userItem.isArmoryItem);
const isCanAddedToClanArmory = computed(
  () => canAddedToClanArmory(userItem.item.type) && !userItem.isPersonal
);
</script>

<template>
  <ItemDetail
    :item="userItem.item"
    :compareResult="compareResult"
    :class="{ 'bg-primary-hover/15': userItem.isPersonal }"
  >
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

      <template v-if="userItem.isArmoryItem">
        <ClanArmoryItemRelationBadge v-if="lender && lender.id !== user!.id" :lender="lender" />
        <Tag
          v-else
          rounded
          size="lg"
          variant="primary"
          icon="armory"
          class="cursor-default opacity-80 hover:opacity-100"
          v-tooltip="$t('character.inventory.item.clanArmory.inArmory.title')"
        />
      </template>
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
              :userItem="userItem"
              @upgrade="emit('upgrade')"
              @reforge="emit('reforge')"
            />
          </div>
        </template>
      </Modal>

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
              icon-left="armory"
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
