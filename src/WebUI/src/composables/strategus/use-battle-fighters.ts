import { getBattleFighters } from '~/services/strategus-service/battle-service'

export const useBattleFighters = () => {
  const { execute: loadBattleFighters, state: battleFighters } = useAsyncState(
    ({ id }: { id: number }) => getBattleFighters(id),
    [],
    {
      immediate: false,
    },
  )

  const battleFightersCount = computed(() => battleFighters.value.length)

  return {
    battleFighters,
    battleFightersCount,
    loadBattleFighters,
  }
}
