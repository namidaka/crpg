import type { Position } from 'geojson'

import { BattlePhase } from '~/models/strategus/battle'
import { getBattles } from '~/services/strategus-service/battle-service'
import { shouldDisplaySettlement } from '~/services/strategus-service/map'
import { positionToLatLng } from '~/utils/geometry'

export const useBattles = () => {
  const route = useRoute()
  const router = useRouter()

  const battlePhaseModel = computed({
    get() {
      return (route.query?.battlePhase as BattlePhase[]) || [BattlePhase.Scheduled, BattlePhase.Hiring]
    },

    set(battlePhases: BattlePhase[]) {
      router.replace({
        query: {
          ...route.query,
          battlePhases,
        },
      })
    },
  })

  const battlePhases = Object.values(BattlePhase)

  return {
    battlePhaseModel,
    battlePhases,
  }
}
