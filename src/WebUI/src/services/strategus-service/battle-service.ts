import type { Region } from '~/models/region'
import type {
  Battle,
  BattleFighter,
  BattleMercenary,
  BattlePhase,
  BattleSide,
} from '~/models/strategus/battle'
import type { Party } from '~/models/strategus/party'
import type { SettlementPublic } from '~/models/strategus/settlement'

import { del, get, post, put } from '~/services/crpg-client'

export const getBattles = async (
  region: Region,
  phases: BattlePhase[],
) => {
  const params = new URLSearchParams()
  params.append('region', region)
  phases.forEach(p => params.append('phase[]', p))
  return await get<Battle[]>(`/battles?${params}`)
}

export const getBattle = async (
  id: number,
) => {
  return await get<Battle>(`/battles/${id}`)
}

export const getBattleFighters = async (id: number) => get<BattleFighter[]>(`/battles/${id}/fighters`)

export const getBattleMercenaries = async (id: number) => get<BattleMercenary[]>(`/battles/${id}/mercenaries`)
