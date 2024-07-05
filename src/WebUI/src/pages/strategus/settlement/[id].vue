<script setup lang="ts">
import { getSettlement } from '@/services/strategus-service/settlement';
import { settlementKey } from '@/symbols/strategus/settlement';
import { useUserStore } from '@/stores/user';

definePage({
  meta: {
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const { user } = toRefs(useUserStore());

const route = useRoute<'StrategusSettlementId'>();

const { state: settlement, execute: loadSettlement } = useAsyncState(
  () => getSettlement(Number(route.params.id)),
  null,
  {
    immediate: false,
  }
);

provide(settlementKey, settlement);

await loadSettlement();
</script>

<template>
  <div
    class="flex h-[95%] w-2/5 flex-col space-y-4 overflow-hidden rounded-3xl bg-base-100 bg-opacity-90 p-6 text-content-200 backdrop-blur-sm"
  >
    <header class="border-b border-border-200 pb-2">
      <!-- TODO: leave from city logic, API -->
      <!-- Exit gate/door icon -->

      <div class="flex items-center gap-5">
        <OButton
          v-tooltip.bottom="`Leave`"
          tag="router-link"
          :to="{ name: 'Strategus' }"
          variant="secondary"
          size="lg"
          outlined
          rounded
          icon-left="arrow-left"
        />

        <div class="flex items-center gap-5">
          <SettlementMedia :settlement="settlement!" />

          <Divider inline />

          <div class="flex items-center gap-1.5" v-tooltip.bottom="`Troops`">
            <OIcon icon="member" size="lg" />
            {{ settlement!.troops }}
          </div>

          <Divider inline />

          <!-- TODO: gold? -->
          <Coin :value="10000" />

          <Divider inline />

          <div v-if="settlement?.owner" class="flex flex-col gap-1">
            <span class="text-3xs text-content-300">Owner</span>
            <UserMedia
              :user="settlement.owner"
              :clan="settlement.owner.clan?.clan"
              :clanRole="settlement.owner.clan?.role"
              :isSelf="settlement.owner.id === user!.id"
              class="max-w-[16rem]"
            />
          </div>
        </div>
      </div>
    </header>

    <nav class="flex items-center justify-center gap-2">
      <RouterLink
        :to="{ name: 'StrategusSettlementId', params: { id: route.params.id } }"
        v-slot="{ isExactActive }"
      >
        <OButton
          :variant="isExactActive ? 'transparent-active' : 'secondary'"
          size="sm"
          :label="`Info`"
        />
      </RouterLink>

      <RouterLink
        :to="{ name: 'StrategusSettlementIdGarrison', params: { id: route.params.id } }"
        v-slot="{ isExactActive }"
      >
        <OButton
          :variant="isExactActive ? 'transparent-active' : 'secondary'"
          size="sm"
          iconLeft="member"
          :label="`Garrison`"
        />
      </RouterLink>
    </nav>

    <div class="h-full overflow-y-auto overflow-x-hidden">
      <RouterView />
    </div>

    <footer class="flex items-center gap-5 border-t border-border-200 pt-2">TODO:</footer>
  </div>
</template>
