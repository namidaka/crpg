<script setup lang="ts">
import { type SettlementPublic } from '@/models/strategus/settlement';
import { settlementIconByType } from '@/services/strategus-service/settlement';

const { settlements } = defineProps<{ settlements: SettlementPublic[] }>();

const emit = defineEmits<{
  select: [position: SettlementPublic['position']['coordinates']];
}>();

const searchSettlement = ref<string>('');
const suggestionsSettlements = computed(() =>
  searchSettlement.value !== ''
    ? settlements.filter(
        settlement =>
          settlement.region === 'Eu' && // TODO: REGION
          settlement.name.toLowerCase().includes(searchSettlement.value.toLowerCase())
      )
    : []
);
</script>

<template>
  <VDropdown
    :triggers="[]"
    :shown="Boolean(suggestionsSettlements.length)"
    noAutoFocus
    :autoHide="false"
    :distance="8"
  >
    <div class="min-w-[14rem]">
      <OInput
        v-model="searchSettlement"
        type="text"
        icon="search"
        rounded
        expanded
        clearable
        size="sm"
        iconRightClickable
      />
    </div>

    <template #popper>
      <div class="max-h-[20rem] min-w-[14rem] overflow-y-auto">
        <DropdownItem
          v-for="settlement in suggestionsSettlements"
          class="text-black"
          @click="$emit('select', settlement.position.coordinates)"
        >
          <OIcon :icon="settlementIconByType[settlement.type].icon" />
          <div>{{ settlement.name }}</div>
        </DropdownItem>
      </div>
    </template>
  </VDropdown>
</template>
