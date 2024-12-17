<script setup lang="ts">
import { useBattleFighters } from '~/composables/strategus/use-battle-fighters'
import { useBattleMercenaries } from '~/composables/strategus/use-battle-mercenaries'
import { useBattle } from '~/composables/strategus/use-battles'
import { BattlePhase, BattleSide } from '~/models/strategus/battle'
import { notify } from '~/services/notification-service'
import { getBattleFighter, getBattles } from '~/services/strategus-service/battle-service'
import { t } from '~/services/translate-service'
import { useUserStore } from '~/stores/user'

const props = defineProps<{
  id: number
}>()

definePage({
  meta: {
    layout: 'default',
    middleware: '', // TODO: ['canManageBattle']
    roles: ['User', 'Moderator', 'Admin'],
  },
  props: true,
})

const userStore = useUserStore()
const router = useRouter()

const { battle, battleId, loadBattle } = useBattle(props.id)

await Promise.all([loadBattle(0, { id: battleId.value })])
</script>

<template>
  <div class="p-6">
    <RouterLink :to="{ name: 'StrategusBattlesId', params: { id: battleId } }">
      <OButton
        v-tooltip.bottom="$t('nav.back')"
        variant="secondary"
        size="xl"
        outlined
        rounded
        icon-left="arrow-left"
      />
    </RouterLink>

    <div class="mx-auto max-w-2xl space-y-10 py-6">
      <div class="space-y-14">
        <h1 class="text-center text-xl text-content-100">
          {{ $t('strategus.battle.manage.title') }}
        </h1>
        Battle settings:
        Auto accept?
        Accept if wage?
        Retreat?
        Abort?
        Manage gear / loadout for troops?
        Remove mercenaries
        <div class="container">
          <div class="mx-auto max-w-3xl" />
        </div>
      </div>
    </div>
  </div>
</template>
