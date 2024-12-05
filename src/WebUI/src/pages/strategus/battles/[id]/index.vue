<script setup lang="ts">
import type { Battle, BattleMercenary } from '~/models/strategus/battle'

import { useBattleFighters } from '~/composables/strategus/use-battle-fighters'
import { useBattleMercenaries } from '~/composables/strategus/use-battle-mercenaries'
import { useBattle } from '~/composables/strategus/use-battles'
import { useLanguages } from '~/composables/use-language'
import { usePagination } from '~/composables/use-pagination'
import { useRegion } from '~/composables/use-region'
import { useSearchDebounced } from '~/composables/use-search-debounce'
import { Culture } from '~/models/culture'
import { BattlePhase, BattleSide } from '~/models/strategus/battle'
import { itemCultureToIcon } from '~/services/item-service' // TODO: culture service
import { getBattles } from '~/services/strategus-service/battle-service'
import { settlementIconByType } from '~/services/strategus-service/settlement'
import { useUserStore } from '~/stores/user'

const props = defineProps<{
  id: number
}>()

const getIconByCulture = (cultureString: string) => itemCultureToIcon[Culture[cultureString as keyof typeof Culture] || Culture.Neutral]
const { battleFightersLoading, battleFighters, battleFightersCount, battleFightersAttackers, battleFightersDefenders, loadBattleFighters } = useBattleFighters()
const { battleMercenariesLoading, battleMercenaries, battleMercenariesCount, battleMercenariesAttackers, battleMercenariesDefenders, loadBattleMercenaries } = useBattleMercenaries()

definePage({
  meta: {
    layout: 'default',
    roles: ['User', 'Moderator', 'Admin'],
  },
  props: true,
})

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()

const { battle, battleId, loadBattle } = useBattle(props.id)

const isSelfUser = (row: BattleMercenary) => row.character.id === userStore.user?.activeCharacterId

const rowClass = (row: BattleMercenary): string =>
  isSelfUser(row) ? 'text-primary' : 'text-content-100'

const attackerMercenarySlots = computed(() => {
  return battleFightersAttackers.value.reduce((total, fighter) => {
    return total + (fighter.mercenarySlots || 0) // Add the value or 0 if it's undefined
  }, 0)
})

const defenderMercenarySlots = computed(() => {
  return battleFightersDefenders.value.reduce((total, fighter) => {
    return total + (fighter.mercenarySlots || 0) // Add the value or 0 if it's undefined
  }, 0)
})

const fetchPageData = async (battleId: number) => {
  await Promise.all([loadBattle(0, { id: battleId }), loadBattleFighters(0, { id: battleId }), loadBattleMercenaries(0, { id: battleId })])
}

await fetchPageData(props.id)
</script>

<template>
  <div v-if="battle !== null" class="pb-12 pt-24">
    <div class="container mb-8">
      <Heading class="mb-5" title="Siege of Epicrotea" />
      <div class="mx-auto mb-16 flex max-w-7xl flex-row gap-x-5">
        <div class="basis-1/6">
          <h1 class="mb-8 text-center text-xl text-content-100">
            {{ $t('Attackers') }}
          </h1>

          <div
            v-for="fighter in battleFightersAttackers"
            :key="fighter.id"
            class="flex flex-col gap-3 pb-4"
          >
            <div v-if="fighter.party?.user">
              <UserMedia
                :user="fighter.party.user"
                hidden-platform
                size="xl"
              />
              commanding: x troops
            </div>
            <div v-if="fighter.settlement?.owner">
              <UserMedia
                :user="fighter.settlement.owner.user"
                hidden-platform
                size="xl"
              />
              commanding: x troops
            </div>
          </div>
        </div>
        <div class="basis-4/6 justify-center">
          <div class="mb-8 flex flex-wrap items-center justify-center gap-4.5">
            <div class="flex items-center gap-1.5">
              <OIcon
                icon="calendar"
                size="lg"
                class="text-content-100"
              />
              <span
                class="text-content-200"
                data-aq-clan-info="tag"
              >{{ $d(battle.scheduledFor, 'date') }}</span>
            </div>

            <div class="h-8 w-px select-none bg-border-200" />

            <div class="flex items-center gap-1.5">
              <OIcon
                icon="clock"
                size="lg"
                class="text-content-100"
              />
              <span
                class="text-content-200"
                data-aq-clan-info="tag"
              >{{ $d(battle.scheduledFor, 'time') }}</span>
            </div>

            <div class="h-8 w-px select-none bg-border-200" />

            <div class="flex items-center gap-1.5">
              <OIcon
                icon="region"
                size="lg"
                class="text-content-100"
              />
              <div
                class="text-content-200"
                data-aq-clan-info="region"
              >
                EU
              </div>
            </div>

            <div class="h-8 w-px select-none bg-border-200" />

            <div class="flex items-center gap-1.5">
              <OIcon
                icon="leader"
                size="lg"
                class="text-content-100"
              />
              <span
                class="text-content-200"
              >
                {{ battleFightersCount }}
              </span>
            </div>

            <div class="h-8 w-px select-none bg-border-200" />

            <div class="flex items-center gap-1.5">
              <OIcon
                icon="member"
                size="lg"
                class="text-content-100"
              />
              <span
                class="text-content-200"
              >
                {{ battleMercenariesCount }}
              </span>
            </div>

            <Divider />
          </div>

          <div class="mb-20 flex items-center justify-center gap-3">
            <OButton
              tag="router-link"
              variant="primary"
              outlined
              size="xl"
            >
              {{ $t('clan.application.title') }}
            </OButton>
          </div>

          <div>
            <div class="grid grid-cols-2">
              <div class="flex flex-row text-base text-white">
                <div>
                  <ClanTagIcon
                    :color="battle.attacker.party?.clan.primaryColor"
                    size="4x"
                  />
                </div>
                <div class="flex grow flex-col">
                  {{ battle.attacker.party?.clan.name }}
                  <div class="text-2xs">
                    <OIcon icon="child" size="sm" />{{ battle.attackerTotalTroops }} <span class="text-base-500">({{ (battle.attackerTotalTroops / (battle.attackerTotalTroops + battle.defenderTotalTroops) * 100).toFixed(2) }} %)</span>
                  </div>
                </div>
              </div>
              <div class="flex flex-row text-right text-base text-white">
                <div class="flex grow flex-col">
                  {{ battle.defender.settlement.owner.clan.name }}
                  <div class="text-2xs">
                    <span class="text-base-500">({{ (battle.defenderTotalTroops / (battle.attackerTotalTroops + battle.defenderTotalTroops) * 100).toFixed(2) }} %) </span>{{ battle.defenderTotalTroops }}<OIcon icon="child" size="sm" />
                  </div>
                </div>
                <div>
                  <ClanTagIcon
                    :color="battle.attacker.party?.clan.primaryColor"
                    size="4x"
                  />
                </div>
              </div>
            </div>
            <div class="my-4 h-2.5 w-full rounded-full bg-base-400">
              <div class="h-2.5 rounded-full bg-base-500" style="width: 5.8%" />
            </div>
            <div class="grid grid-cols-2">
              <div class="inline-flex flex-row gap-1.5 text-base text-white">
                <OIcon
                  icon="member"
                  class="text-content-100"
                />
                {{ battleMercenariesCount }} / {{ attackerMercenarySlots }}
              </div>
              <div class="inline-flex flex-row-reverse gap-1.5 text-base text-white">
                <OIcon
                  icon="member"
                  class="text-content-100"
                />
                {{ battleMercenariesCount }} / {{ defenderMercenarySlots }}
              </div>
            </div>
          </div>
        </div>
        <div class="basis-1/6 text-right">
          <h1 class="mb-8 text-center text-xl text-content-100">
            {{ $t('Defenders') }}
          </h1>
          <div
            v-for="fighter in battleFightersDefenders"
            :key="fighter.id"
            class="flex flex-col gap-3 pb-4"
          >
            <div v-if="fighter.party?.user">
              <UserMedia
                :user="fighter.party.user"
                hidden-platform
                size="xl"
                class="justify-end"
              />
              commanding: x troops
            </div>
            <div v-if="fighter.settlement?.owner">
              <UserMedia
                :user="fighter.settlement.owner.user"
                hidden-platform
                size="xl"
                class="justify-end"
              />
              commanding: x troops
            </div>
          </div>
        </div>
      </div>
      <Divider />
    </div>
  </div>
</template>
