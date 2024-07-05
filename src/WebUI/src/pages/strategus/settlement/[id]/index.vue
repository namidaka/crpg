<script setup lang="ts">
import { settlementKey } from '@/symbols/strategus/settlement';

definePage({
  meta: {
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const settlement = injectStrict(settlementKey);
</script>

<template>
  <div>
    <div class="prose prose-invert">
      <div>TODO: Settlement town center</div>
      <ul>
        <li>--</li>
        <li>--</li>
      </ul>
    </div>
    <!-- <OButton
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
        /> -->
  </div>
</template>
