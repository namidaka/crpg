import type { GameServerModeStats, GameServerRegionStats, GameServerStats } from '~/models/game-server-stats'

import { get } from '~/services/crpg-client'

// TODO: move to backend
const omitEmptyGameServerStats = (regions: GameServerRegionStats): GameServerRegionStats => Object.fromEntries(
  Object.entries(regions)
    .map(([region, gameModes]) => {
      const filteredModes = Object.fromEntries(
        Object.entries(gameModes).filter(
          ([_, modeData]) => modeData.playingCount > 0,
        ),
      )

      return Object.keys(filteredModes).length > 0
        ? [region, filteredModes]
        : null
    })
    .filter((region): region is [string, GameServerModeStats] => region !== null),
)

export const getGameServerStats = async () => {
  const { regions, total } = await get<GameServerStats>('/game-server-statistics')
  return {
    total,
    regions: omitEmptyGameServerStats(regions),
  }
}
