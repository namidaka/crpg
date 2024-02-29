import { GameMode } from '@/models/game-mode';




export const useGameMode = () => {
  const route = useRoute();
  const router = useRouter();

  const gameModeModel = computed({
    get() {
      return (route.query?.gameMode as GameMode) || GameMode.Battle;
    },

    set(gameMode: GameMode) {
      router.replace({
        query: {
          ...route.query,
          gameMode,
        },
      });
    },
  });

  const gameModes = Object.values(GameMode);
  const rankedGameModes = [ GameMode.Battle, GameMode.Duel];

  return {
    gameModeModel,
    gameModes,
    rankedGameModes,
  };
};
