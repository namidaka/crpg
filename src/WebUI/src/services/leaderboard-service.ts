import qs from 'qs'

import type { CharacterClass } from '~/models/character'
import type { ClanEdition } from '~/models/clan'
import type {
  CharacterCompetitive,
  CharacterCompetitiveNumbered,
  Rank,
} from '~/models/competitive'
import type { GameMode } from '~/models/game-mode'
import type { Region } from '~/models/region'
import type { UserPublic } from '~/models/user'

import { RankGroup } from '~/models/competitive'
import { mapClanResponse } from '~/services/clan-service'
import { get } from '~/services/crpg-client'
import { inRange } from '~/utils/math'
import { getEntries } from '~/utils/object'

interface UserPublicRaw extends Omit<UserPublic, 'clan'> {
  clan: ClanEdition | null
}

interface CharacterCompetitiveRaw extends Omit<CharacterCompetitive, 'user'> {
  user: UserPublicRaw
}

export const getLeaderBoard = async ({
  characterClass,
  gameMode,
  region,
}: {
  region?: Region
  characterClass?: CharacterClass
  gameMode?: GameMode
}): Promise<CharacterCompetitiveNumbered[]> => {
  const params = qs.stringify(
    { characterClass, gameMode, region },
    {
      arrayFormat: 'brackets',
      skipNulls: true,
      strictNullHandling: true,
    },
  )

  const res = await get<CharacterCompetitiveRaw[]>(`/leaderboard/leaderboard?${params}`)

  return res.map((cr, idx) => ({
    ...cr,
    position: idx + 1,
    user: {
      ...cr.user,
      clan: cr.user.clan === null ? null : mapClanResponse(cr.user.clan),
    },
  }))
}

const rankColors: Record<RankGroup, string> = {
  [RankGroup.Bronze]: '#CC6633',
  [RankGroup.Champion]: '#B73E6C',
  [RankGroup.Copper]: '#B87333',
  [RankGroup.Diamond]: '#C289F5',
  [RankGroup.Gold]: '#EABC40',
  [RankGroup.Iron]: '#A19D94',
  [RankGroup.Platinum]: '#40A7B9',
  [RankGroup.Silver]: '#C7CCCA',
}

const step = 50
const rankSubGroupCount = 5

const createRank = (baseRank: [RankGroup, string]) =>
  [...Array.from({ length: rankSubGroupCount }).keys()].reverse().map(subRank => ({
    color: baseRank[1],
    groupTitle: baseRank[0],
    title: `${baseRank[0]} ${subRank + 1}`,
  }))

export const createRankTable = (): Rank[] =>
  getEntries<Record<RankGroup, string>>(rankColors)
    .flatMap(createRank)
    .map((baseRank, idx) => ({ ...baseRank, max: idx * step + step, min: idx * step }))

export const getRankByCompetitiveValue = (rankTable: Rank[], competitiveValue: number) => {
  if (competitiveValue < rankTable[0].min) {
    return rankTable[0]
  }

  if (competitiveValue > rankTable[rankTable.length - 1].max) {
    return rankTable[rankTable.length - 1]
  }

  return createRankTable().find(row => inRange(competitiveValue, row.min, row.max))!
}
