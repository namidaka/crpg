import { getGameServerStats } from '@/services/game-server-statistics-service';
import { usePollInterval } from '@/composables/use-poll-interval';

export const useGameServerStats = () => {
  const { state: gameServerStats, execute: loadGameServerStats } = useAsyncState(
    () => getGameServerStats(),
    {
      total: { playingCount: 0 },
      regions: {},
    },
    {
      immediate: false,
    }
  );

  const { subscribe, unsubscribe } = usePollInterval();
  const id = Symbol('loadGameServerStats');
  onMounted(() => {
    subscribe(id, loadGameServerStats);
  });
  onBeforeUnmount(() => {
    unsubscribe(id);
  });

  return {
    gameServerStats,
    loadGameServerStats,
  };
};
