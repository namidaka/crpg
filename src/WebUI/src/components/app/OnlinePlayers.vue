<script setup lang="ts">
import { useTransition } from '@vueuse/core';
import { type GameServerStats } from '@/models/game-server-stats';
import { n } from '@/services/translate-service';
import { gameModeToIcon } from '@/services/game-mode-service';
import { omitEmptyGameMode } from '@/services/game-server-statistics-service';

import { getEntries } from '@/utils/object';

const gameStatsErrorIndicator = '?';

const { gameServerStats, showLabel = false } = defineProps<{
  gameServerStats: GameServerStats | null;
  showLabel?: boolean;
}>();

const animatedPlayingCount = useTransition(
  toRef(() => (gameServerStats !== null ? gameServerStats.total.playingCount : -1))
);

const animatedPlayingCountString = computed(() =>
  gameServerStats !== null
    ? n(Number(animatedPlayingCount.value.toFixed(0)), 'decimal')
    : gameStatsErrorIndicator
);

const omittedEmptyGameServerStats = computed(() => {
  if (gameServerStats === null) return {};
  return getEntries(gameServerStats.regions).reduce(
    (out, [region, gameModes]) => {
      out[region] = omitEmptyGameMode(gameModes!);
      return out;
    },
    {} as GameServerStats['regions']
  );
});
</script>

<template>
  <VTooltip
    :disabled="gameServerStats === null || Object.keys(gameServerStats.regions).length === 0"
  >
    <div class="flex select-none items-center gap-1.5 hover:text-content-100">
      <FontAwesomeLayers class="fa-lg">
        <FontAwesomeIcon class="text-[#53BC96]" :icon="['crpg', 'online']" />
        <FontAwesomeIcon
          class="animate-ping text-[#53BC96] text-opacity-15"
          :icon="['crpg', 'online-ring']"
        />
      </FontAwesomeLayers>
      <div v-if="showLabel" data-aq-online-players-count>
        {{ $t('onlinePlayers.format', { count: animatedPlayingCountString }) }}
      </div>
      <div v-else class="w-8" data-aq-online-players-count>
        {{ animatedPlayingCountString }}
      </div>
    </div>
    <template #popper>
      <div class="prose prose-invert space-y-5">
        <h5 class="text-content-100">{{ $t('onlinePlayers.tooltip.title') }}</h5>
        <div class="space-y-6" v-if="gameServerStats !== null" data-aq-region-stats>
          <div
            v-for="(regionServerStats, regionKey) in omittedEmptyGameServerStats"
            class="flex flex-col gap-3"
          >
            <div class="text-white">{{ $t(`region.${regionKey}`, 0) }}</div>
            <div
              v-for="(regionServerMode, gameModeKey) in regionServerStats"
              class="flex w-44 items-center justify-between gap-2"
            >
              <div class="flex items-center gap-2">
                <OIcon size="xl" :icon="gameModeToIcon[gameModeKey]" />
                <div>{{ $t(`game-mode.${gameModeKey}`) }}</div>
              </div>
              <div class="text-white">{{ regionServerMode!.playingCount }}</div>
            </div>
          </div>
        </div>
      </div>
    </template>
  </VTooltip>
</template>
