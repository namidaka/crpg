<script setup lang="ts">
import { useUserStore } from '~/stores/user'

const { inInventoryCount = 0, notEnoughGold, price, upkeep } = defineProps<{
  price: number
  upkeep: number
  inInventoryCount: number
  notEnoughGold: boolean
}>()

defineEmits<{
  buy: []
}>()

const { user } = toRefs(useUserStore())

const isExpensive = computed(() => user.value!.gold - price < upkeep)
</script>

<template>
  <VTooltip>
    <div>
      <OButton
        variant="primary"
        outlined
        size="lg"
        :disabled="notEnoughGold"
        @click="$emit('buy')"
      >
        <Coin
          :value="price"
          :class="{ 'opacity-50': notEnoughGold }"
        />
        <Tag
          v-if="inInventoryCount > 0"
          size="sm"
          variant="primary"
          rounded
          :label="String(inInventoryCount)"
        />
        <Tag
          v-if="isExpensive"
          icon="alert"
          size="sm"
          variant="warning"
          rounded
        />
      </OButton>
    </div>

    <template #popper>
      <div class="prose prose-invert space-y-4">
        <h5>{{ $t('shop.item.buy.tooltip.buy') }}</h5>

        <div class="item-center flex gap-2">
          {{ $t('item.aggregations.upkeep.title') }}:
          <Coin>
            {{ $t('item.format.upkeep', { upkeep: $n(upkeep as number) }) }}
          </Coin>
        </div>

        <p
          v-if="inInventoryCount > 0"
          class="text-primary"
        >
          {{ $t('shop.item.buy.tooltip.inInventory', { count: inInventoryCount }) }}
        </p>
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
    </template>
  </VTooltip>
</template>
