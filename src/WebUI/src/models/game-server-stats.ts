import { Region } from './region';
import { GameMode } from './game-mode';

interface GameStats {
  playingCount: number;
}

export type GameServerModeStats = Partial<Record<GameMode, GameStats>>;

export type GameServerRegionStats = Partial<Record<Region, GameServerModeStats>>;

export interface GameServerStats {
  total: GameStats;
  regions: GameServerRegionStats;
}
