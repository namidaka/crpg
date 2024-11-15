import type { CharacterClass, CharacterStatistics } from '~/models/character'
import type { UserPublic } from '~/models/user'

export interface CharacterCompetitive {
  id: number
  level: number
  user: UserPublic
  class: CharacterClass
  statistics: CharacterStatistics[]
}

export interface CharacterCompetitiveNumbered extends CharacterCompetitive {
  position: number
}

export interface Rank {
  min: number
  max: number
  title: string
  color: string
  groupTitle: string
}

export enum RankGroup {
  Iron = 'Iron',
  Copper = 'Copper',
  Bronze = 'Bronze',
  Silver = 'Silver',
  Gold = 'Gold',
  Platinum = 'Platinum',
  Diamond = 'Diamond',
  Champion = 'Champion',
}
