import { mockGet } from 'vi-fetch'

import type { GameServerStats } from '~/models/game-server-stats'

import { response } from '~/__mocks__/crpg-client'
import { GameMode } from '~/models/game-mode'
import { Region } from '~/models/region'
import { getGameServerStats } from '~/services/game-server-statistics-service'

it('getGameServerStats', async () => {
  const mockServerStats: GameServerStats = {
    total: {
      playingCount: 12,
    },
    regions: {
      [Region.Eu]: {
        [GameMode.Battle]: {
          playingCount: 12,
        },
        [GameMode.DTV]: {
          playingCount: 0,
        },
      },
      [Region.Na]: {
        [GameMode.DTV]: {
          playingCount: 0,
        },
      },
    },
  }

  mockGet('/game-server-statistics').willResolve(
    response(mockServerStats),
  )

  expect(await getGameServerStats()).toEqual({
    total: {
      playingCount: 12,
    },
    regions: {
      [Region.Eu]: {
        [GameMode.Battle]: {
          playingCount: 12,
        },
      },
    },
  })
})
