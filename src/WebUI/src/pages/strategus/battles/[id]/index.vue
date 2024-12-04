<script setup lang="ts">
import { useBattleFighters } from '~/composables/strategus/use-battle-fighters'
import { useBattle } from '~/composables/strategus/use-battles'
import { useLanguages } from '~/composables/use-language'
import { usePagination } from '~/composables/use-pagination'
import { useRegion } from '~/composables/use-region'
import { useSearchDebounced } from '~/composables/use-search-debounce'
import { Culture } from '~/models/culture'
import { type Battle, BattlePhase } from '~/models/strategus/battle'
import { itemCultureToIcon } from '~/services/item-service' // TODO: culture service
import { getBattles } from '~/services/strategus-service/battle-service'
import { settlementIconByType } from '~/services/strategus-service/settlement'
import { useUserStore } from '~/stores/user'

const props = defineProps<{
  id: number
}>()

const getIconByCulture = (cultureString: string) => itemCultureToIcon[Culture[cultureString as keyof typeof Culture] || Culture.Neutral]
const { battleFighters, battleFightersCount, loadBattleFighters } = useBattleFighters()

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

const fetchPageData = async (battleId: number) => {
  await Promise.all([loadBattle(0, { id: battleId }), loadBattleFighters(0, { id: battleId })])
}

await fetchPageData(props.id)
</script>

<template>
  <div v-if="battle !== null" class="pb-12 pt-24">
    <div class="container mb-8">
      <Heading class="mb-5" title="Siege of Epicrotea" />
      <div class="mx-auto flex max-w-7xl flex-row gap-x-5">
        <div class="basis-1/6">
          <div
            v-for="fighter in battleFighters"
            :key="fighter.id"
            class="flex flex-col gap-3"
          >
            {{ fighter.party?.user.name }}
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
                icon="member"
                size="lg"
                class="text-content-100"
              />
              <span
                class="text-content-200"
                data-aq-clan-info="member-count"
              >
                {{ battleFightersCount }}
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
                    <OIcon icon="child" size="sm" />500 <span class="text-base-500">(5.8%)</span>
                  </div>
                </div>
              </div>
              <div class="flex flex-row text-right text-base text-white">
                <div class="flex grow flex-col">
                  {{ battle.defender.settlement.owner.clan.name }}
                  <div class="text-2xs">
                    <span class="text-base-500">(94.2%) </span>8000<OIcon icon="child" size="sm" />
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
          </div>
        </div>
        <div class="basis-1/6 text-right">
          <div>{{ battle.defender?.settlement?.name }} <OIcon v-tooltip="$t(`strategus.settlementType.${battle.defender!.settlement.type}`)" :icon="settlementIconByType[battle.defender!.settlement.type].icon" class="self-baseline" /></div>
          <div>{{ battle.defenderTotalTroops }} Troops</div>
          Commanded by:
          <UserMedia
            class="justify-end"
            :user="battle.defender?.settlement?.owner?.user"
            hidden-platform
            size="xl"
          />
        </div>
      </div>
    </div>
  </div>
</template>
