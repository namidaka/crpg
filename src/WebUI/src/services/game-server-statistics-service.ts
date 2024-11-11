import { omitBy } from 'es-toolkit'

import type { GameServerModeStats, GameServerStats } from '~/models/game-server-stats'

import { get } from '~/services/crpg-client'

export const getGameServerStats = () => get<GameServerStats>('/game-server-statistics')

// TODO: omit immediately, or better yet, on the backend side.
export const omitEmptyGameMode = (gameModes: GameServerModeStats) => omitBy(gameModes, stats => (stats?.playingCount || 0) < 0)
