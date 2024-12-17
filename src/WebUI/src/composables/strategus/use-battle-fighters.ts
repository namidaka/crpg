import { BattleSide } from '~/models/strategus/battle'
import { getBattleFighters } from '~/services/strategus-service/battle-service'

export const useBattleFighters = () => {
  const { execute: loadBattleFighters, isLoading: battleFightersLoading, state: battleFighters } = useAsyncState(
    ({ id }: { id: number }) => getBattleFighters(id),
    [],
    {
      immediate: false,
    },
  )

  const battleFightersCount = computed(() => battleFighters.value.length)

  const battleFightersAttackers = computed(() =>
    battleFighters.value.filter(fighter => fighter.side === BattleSide.Attacker && !fighter.commander))

  const battleFightersDefenders = computed(() =>
    battleFighters.value.filter(fighter => fighter.side === BattleSide.Defender && !fighter.commander))

  return {
    battleFightersLoading,
    battleFighters,
    battleFightersCount,
    battleFightersAttackers,
    battleFightersDefenders,
    loadBattleFighters,
  }
}
