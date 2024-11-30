<script setup lang="ts">
import type { ItemFlat, ItemRank } from '~/models/item'
import type { UserItem } from '~/models/user'

import { useItemReforge } from '~/composables/item/use-item-reforge'
import { useItemUpgrades } from '~/composables/item/use-item-upgrades'
import { ItemCompareMode } from '~/models/item'
import { createItemIndex } from '~/services/item-search-service/indexator'
import { getItemAggregations, getRankColor } from '~/services/item-service'
import { useUserStore } from '~/stores/user'

const { userItem } = defineProps<{
  userItem: UserItem
}>()

const emit = defineEmits<{
  upgrade: []
  reforge: []
}>()

const userStore = useUserStore()

const item = computed(() => createItemIndex([userItem.item])[0])
const aggregationsConfig = computed(() => getItemAggregations(item.value))

const {
  baseItem,
  canUpgrade,
  isLoading,
  itemUpgrades,
  nextItem,
  relativeEntries,
  validation: upgradeValidation,
} = useItemUpgrades(item.value, aggregationsConfig.value)

const {
  canReforge,
  reforgeCost,
  reforgeCostTable,
  validation: reforgeValidation,
} = useItemReforge(item.value)
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between gap-12 pl-4 pr-16">
      <div class="flex items-center gap-4">
        <h3 class="text-xl font-semibold">
          {{ $t('character.inventory.item.upgrade.upgradesTitle') }}
        </h3>
        <Loom :point="userStore.user!.heirloomPoints" />
        <Coin :value="userStore.user!.gold" />
      </div>

      <div class="flex items-center gap-4">
        <Modal :disabled="!canUpgrade">
          <VTooltip>
            <div>
              <OButton
                variant="primary"
                outlined
                size="lg"
                :disabled="!canUpgrade"
              >
                {{ $t('action.upgrade') }}
                <Loom :point="1" />
              </OButton>
            </div>
            <template #popper>
              <div class="prose prose-invert">
                <h5>
                  {{ $t('character.inventory.item.upgrade.tooltip.title') }}
                </h5>
                <p>
                  {{ $t('character.inventory.item.upgrade.tooltip.description') }}
                </p>
                <i18n-t
                  v-if="!upgradeValidation.maxRank"
                  scope="global"
                  keypath="character.inventory.item.upgrade.validation.maxRank"
                  class="text-status-danger"
                  tag="p"
                />
                <i18n-t
                  v-else-if="!upgradeValidation.points"
                  scope="global"
                  keypath="character.inventory.item.upgrade.validation.loomPoints"
                  class="text-status-danger"
                  tag="p"
                />
              </div>
            </template>
          </VTooltip>

          <template #popper="{ hide }">
            <ConfirmActionForm
              :title="$t('action.confirmation')"
              name="Upgrade item"
              :confirm-label="$t('action.confirm')"
              @cancel="hide"
              @confirm="
                () => {
                  hide();
                  emit('upgrade');
                }
              "
            >
              <template #description>
                <i18n-t
                  scope="global"
                  keypath="character.inventory.item.upgrade.confirm.description"
                  tag="div"
                >
                  <template #loomPoints>
                    <Loom :point="1" />
                  </template>
                  <template #oldItem>
                    <span
                      class="font-bold"
                      :style="{ color: getRankColor(item.rank) }"
                    >
                      {{ item.name }}
                    </span>
                  </template>
                  <template #newItem>
                    <span
                      class="font-bold"
                      :style="{ color: getRankColor(nextItem.rank) }"
                    >
                      {{ nextItem.name }}
                    </span>
                  </template>
                </i18n-t>
              </template>
            </ConfirmActionForm>
          </template>
        </Modal>

        <Modal :disabled="!canReforge">
          <VTooltip>
            <div>
              <OButton
                variant="primary"
                outlined
                size="lg"
                :disabled="!canReforge"
              >
                {{ $t('action.reforge') }}
                <Coin
                  v-if="reforgeValidation.rank"
                  :value="reforgeCost"
                />
              </OButton>
            </div>

            <template #popper>
              <div class="prose prose-invert">
                <h5>
                  {{ $t('character.inventory.item.reforge.tooltip.title') }}
                </h5>
                <p>
                  {{ $t('character.inventory.item.reforge.tooltip.description') }}
                </p>
                <OTable
                  :data="reforgeCostTable"
                  bordered
                  narrowed
                  :loading="isLoading"
                >
                  <OTableColumn
                    v-slot="{ row }: { row: [string, number] }"
                    field="rank"
                    :label="
                      $t('character.inventory.item.reforge.tooltip.costTable.cols.rank.label')
                    "
                  >
                    <span :style="{ color: getRankColor(Number(row[0]) as ItemRank) }">
                      +{{ row[0] }}
                    </span>
                  </OTableColumn>
                  <OTableColumn
                    v-slot="{ row }: { row: [string, number] }"
                    field="rank"
                    :label="
                      $t('character.inventory.item.reforge.tooltip.costTable.cols.cost.label')
                    "
                  >
                    <Coin :value="row[1]" />
                  </OTableColumn>
                  <OTableColumn
                    v-slot="{ row }: { row: [string, number] }"
                    field="looms"
                    :label="
                      $t('character.inventory.item.reforge.tooltip.costTable.cols.looms.label')
                    "
                  >
                    <Loom :point="Number(row[0])" />
                  </OTableColumn>
                </OTable>

                <i18n-t
                  v-if="!reforgeValidation.rank"
                  scope="global"
                  keypath="character.inventory.item.reforge.validation.rank"
                  class="text-status-danger"
                  tag="p"
                >
                  <template #minimumRank>
                    <span class="font-bold">0</span>
                  </template>
                </i18n-t>
                <i18n-t
                  v-else-if="!reforgeValidation.gold"
                  scope="global"
                  keypath="character.inventory.item.reforge.validation.gold"
                  class="text-status-danger"
                  tag="p"
                />
              </div>
            </template>
          </VTooltip>

          <template #popper="{ hide }">
            <ConfirmActionForm
              :title="$t('action.confirmation')"
              name="Reforge item"
              :confirm-label="$t('action.confirm')"
              @cancel="hide"
              @confirm="
                () => {
                  hide();
                  emit('reforge');
                }
              "
            >
              <template #description>
                <i18n-t
                  scope="global"
                  keypath="character.inventory.item.reforge.confirm.description"
                  tag="div"
                >
                  <template #gold>
                    <Coin :value="reforgeCost" />
                  </template>
                  <template #loomPoints>
                    <Loom :point="item.rank" />
                  </template>
                  <template #oldItem>
                    <span
                      class="font-bold"
                      :style="{ color: getRankColor(item.rank) }"
                    >
                      {{ item.name }}
                    </span>
                  </template>
                  <template #newItem>
                    <span
                      class="font-bold"
                      :style="{ color: getRankColor(baseItem.rank) }"
                    >
                      {{ baseItem.name }}
                    </span>
                  </template>
                </i18n-t>
              </template>
            </ConfirmActionForm>
          </template>
        </Modal>
      </div>
    </div>

    <OTable
      :data="itemUpgrades"
      bordered
      narrowed
      hoverable
      :selected="item"
      custom-row-key="id"
    >
      <OTableColumn
        v-slot="{ row: rowItem }: { row: ItemFlat }"
        field="name"
      >
        <div class="relative">
          <ShopGridItemName :item="rowItem">
            <template
              v-if="item?.id === rowItem.id"
              #bottom-right
            >
              <Tag
                v-tooltip="$t('character.inventory.item.upgrade.currentItem')"
                rounded
                variant="primary"
                icon="check"
              />
            </template>
          </ShopGridItemName>
        </div>
      </OTableColumn>

      <OTableColumn
        v-for="(field, idx) in Object.keys(aggregationsConfig) as Array<keyof ItemFlat>"
        :key="idx"
        v-slot="{ row: rowItem }: { row: ItemFlat }"
        :field="field"
        :label="$t(`item.aggregations.${field}.title`)"
        :width="120"
      >
        <ItemParam
          :item="rowItem"
          :field="field"
          is-compare
          :compare-mode="ItemCompareMode.Relative"
          :relative-value="relativeEntries[field]"
        />
      </OTableColumn>

      <template #empty>
        <div class="relative min-h-40">
          <OLoading
            active
            icon-size="xl"
            :full-page="false"
          />
        </div>
      </template>
    </OTable>
  </div>
</template>
