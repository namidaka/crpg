import type { Point } from 'geojson'

import type { Character } from '~/models/character'
import type { Region } from '~/models/region'
import type { PartyCommon } from '~/models/strategus/party'
import type { SettlementPublic } from '~/models/strategus/settlement'
import type { UserPublic } from '~/models/user'

export enum BattlePhase {
  Preparation = 'Preparation',
  Hiring = 'Hiring',
  Scheduled = 'Scheduled',
  Live = 'Live',
  End = 'End',
}

export enum BattleSide {
  Attacker = 'Attacker',
  Defender = 'Defender',
}

export interface Battle {
  id: number
  phase: BattlePhase
  region: Region
  position: Point
  scheduledFor: Date | null
  attacker: BattleFighter
  attackerTotalTroops: number
  defender: BattleFighter | null
  defenderTotalTroops: number
}

export interface BattleFighter {
  id: number
  commander: boolean
  party: PartyCommon | null
  settlement: SettlementPublic | null
  side: BattleSide
  mercenarySlots: number
}

export enum BattleFighterApplicationStatus {
  Pending = 'Pending',
  Declined = 'Declined',
  Accepted = 'Accepted',
}

export interface BattleFighterApplication {
  id: number
  party: PartyCommon
  battleSide: BattleSide
  status: BattleFighterApplicationStatus
}

export interface BattleMercenary {
  user: UserPublic
  character: Character
  captain: BattleFighter
  side: BattleSide
}

export enum BattleMercenaryApplicationStatus {
  Pending = 'Pending',
  Declined = 'Declined',
  Accepted = 'Accepted',
}

export interface BattleMercenaryApplication {
  id: number
  user: UserPublic
  character: Character
  battleSide: BattleSide
  wage: number
  note: string
  status: BattleMercenaryApplicationStatus
}
