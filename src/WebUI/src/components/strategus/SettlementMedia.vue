<script setup lang="ts">
import type { SettlementPublic } from '~/models/strategus/settlement'

import { settlementIconByType } from '~/services/strategus-service/settlement'

const { settlement } = defineProps<{ settlement: SettlementPublic }>()

const settlementIcon = computed(() => settlementIconByType[settlement.type])
</script>

<template>
  <DropdownItem>
    <OIcon :icon="settlementIcon.icon" />
    <div>{{ settlement.name }}</div>
  </DropdownItem>
</template>
