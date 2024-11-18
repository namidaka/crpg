<script setup lang="ts">
import { useUserStore } from '~/stores/user'

const { inInventory, notEnoughGold, price, upkeep } = defineProps<{
  price: number
  upkeep: number
  inInventory: boolean
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
          v-if="inInventory"
          icon="check"
          size="sm"
          variant="success"
          rounded
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
          v-if="inInventory"
          class="text-status-success"
        >
          {{ $t('shop.item.buy.tooltip.inInventory') }}
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
