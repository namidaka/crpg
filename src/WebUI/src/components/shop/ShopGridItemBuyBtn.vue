<script setup lang="ts">
import { groupBy } from 'es-toolkit'

import type { UserItem } from '~/models/user'

import { getRankColor } from '~/services/item-service'
import { useUserStore } from '~/stores/user'

const { inInventoryItems = [], notEnoughGold, price, upkeep } = defineProps<{
  price: number
  upkeep: number
  inInventoryItems: UserItem[]
  notEnoughGold: boolean
}>()

defineEmits<{
  buy: []
}>()

const { user } = toRefs(useUserStore())

const groupedByRankInventoryItems = computed(() => groupBy(inInventoryItems, ui => ui.item.rank))

const isExpensive = computed(() => user.value!.gold - price < upkeep)
</script>

<template>
  <VTooltip placement="left-start" :triggers="['click']">
    <template #default>
      <OButton
        variant="primary"
        outlined
        size="lg"
        :disabled="notEnoughGold"
      >
        <Coin
          :value="price"
          :class="{ 'opacity-50': notEnoughGold }"
        />
        <Tag
          v-if="inInventoryItems.length > 0"
          size="sm"
          variant="primary"
          rounded
          :label="String(inInventoryItems.length)"
        />
        <Tag
          v-if="isExpensive"
          icon="alert"
          size="sm"
          variant="warning"
          rounded
        />
      </OButton>
    </template>

    <template #popper="{ hide }">
      <div class="space-y-4">
        <div class="prose prose-invert space-y-4">
          <h5>{{ $t('shop.item.buy.tooltip.buy') }}</h5>

          <div class="flex items-center gap-2">
            {{ $t('item.aggregations.upkeep.title') }}:
            <Coin>
              {{ $t('item.format.upkeep', { upkeep: $n(upkeep as number) }) }}
            </Coin>
          </div>

          <i18n-t
            v-if="inInventoryItems.length > 0"
            scope="global"
            keypath="shop.item.buy.tooltip.inInventory"
            tag="p"
            class="leading-relaxed"
          >
            <template #items>
              <div
                v-for="(items, group, idx) in groupedByRankInventoryItems" :key="group"
                class="inline"
              >
                <span class="font-semibold" :style="{ color: getRankColor(items[0].item.rank) }">{{ items[0].item.name }} ({{ items.length }})</span>

                <!-- eslint-disable-next-line vue/singleline-html-element-content-newline -->
                <template v-if="idx + 1 < Object.keys(groupedByRankInventoryItems).length">, </template>
              </div>
            </template>
          </i18n-t>

          <p
            v-if="notEnoughGold"
            class="text-status-danger"
          >
            {{ $t('shop.item.buy.tooltip.notEnoughGold') }}
          </p>
          <p
            v-if="isExpensive"
            class="text-status-warning"
          >
            {{ $t('shop.item.expensive') }}
          </p>
        </div>

        <Divider />

        <div class="flex items-center justify-center gap-2">
          <OButton
            variant="success"
            size="2xs"
            icon-left="check"
            :label="$t('shop.item.buy.tooltip.buy')"
            @click="
              () => {
                $emit('buy')
                hide()
              }
            "
          />
          <OButton
            variant="danger"
            size="2xs"
            icon-left="close"
            :label="$t('action.cancel')"
            @click="hide"
          />
        </div>
      </div>
    </template>
  </VTooltip>
</template>
