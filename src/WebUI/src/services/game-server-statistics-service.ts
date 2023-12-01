import { get } from '@/services/crpg-client';
import { GameServerStats, GameServerModeStats } from '@/models/game-server-stats';
import { omitPredicate } from '@/utils/object';

export const getGameServerStats = () => get<GameServerStats>('/game-server-statistics');

export const omitEmptyGameMode = (gameModes: GameServerModeStats) =>
  omitPredicate(gameModes, d => gameModes[d]!.playingCount > 0);
