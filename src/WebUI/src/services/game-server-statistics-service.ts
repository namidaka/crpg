import { omitBy } from 'es-toolkit'

import type { GameServerModeStats, GameServerStats } from '~/models/game-server-stats'

import { get } from '~/services/crpg-client'

export const getGameServerStats = () => get<GameServerStats>('/game-server-statistics')

export const omitEmptyGameMode = (gameModes: GameServerModeStats) =>
  omitBy(gameModes, d => (d?.playingCount || 0) > 0)
