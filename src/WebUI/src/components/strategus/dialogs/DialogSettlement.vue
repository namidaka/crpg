<script setup lang="ts">
import { type Party, PartyStatus } from '@/models/strategus/party';
import { getSettlementGarrisonItems } from '@/services/strategus-service/settlement';

const { party } = defineProps<{ party: Party; isTogglingRecruitTroops: boolean }>();

// TODO: to composable

defineEmits<{
  toggleRecruitTroops: [];
}>();

const settlement = computed(() => party.targetedSettlement!); // TODO: from props

const { state: garrisonItems, execute: loadGarrisonItems } = useAsyncState(
  () => getSettlementGarrisonItems(settlement.value.id),
  [],
  {
    immediate: false,
    resetOnExecute: false,
  }
);

await loadGarrisonItems();
</script>

<template>
  <DialogBase>
    <div class="space-y-8">
      <div class="prose prose-invert">
        <h2>{{ settlement.name }}</h2>
        <p>Culture: {{ settlement.culture }}</p>
        <p>Garrison: {{ settlement.troops }}</p>
        <p>Owner: {{ settlement.troops }}</p>

        <br />
        <br />

        <p>Settlement owner</p>

        <div>
          {{ garrisonItems }}
        </div>

        <br />
        <br />
        <br />
        <br />
        <br />

        <div>Settlement shop (with loomed items??? TODO:)</div>
        <div>Party inventory</div>
        <div>TODO: add item from party inventory</div>
      </div>

      <div class="flex gap-4">
        <OButton
          variant="primary"
          size="lg"
          :label="
            party!.status !== PartyStatus.RecruitingInSettlement
              ? 'Start recruiting troops'
              : 'Stop recruiting troops'
          "
          :loading="isTogglingRecruitTroops"
          :disabled="isTogglingRecruitTroops"
          @click="$emit('toggleRecruitTroops')"
        />

        <OButton variant="primary" size="lg" label="TODO: Shop" />
      </div>
    </div>
  </DialogBase>
</template>
