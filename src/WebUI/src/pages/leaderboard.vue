<script setup lang="ts">
import { type CharacterCompetitiveNumbered } from '@/models/competitive';
import { CharacterClass } from '@/models/character';

import { getLeaderBoard, createRankTable } from '@/services/leaderboard-service';
import { characterClassToIcon } from '@/services/characters-service';
import { useUserStore } from '@/stores/user';
import { useRegion } from '@/composables/use-region';
import { useGameMode } from '@/composables/use-gamemode';
import { GameMode } from '@/models/game-mode';
import { gameModeToIcon } from '@/services/game-mode-service';

definePage({
  meta: {
    layout: 'default',
    bg: 'background-2.webp',
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const route = useRoute();
const router = useRouter();

const userStore = useUserStore();

const { gameModeModel, rankedGameModes } = useGameMode();
const { regionModel, regions } = useRegion();

const characterClassModel = computed({
  get() {
    return (route.query?.class as CharacterClass) || undefined;
  },

  set(characterClass: CharacterClass | undefined) {
    router.replace({
      query: {
        ...route.query,
        class: characterClass,
      },
    });
  },
});

const characterClasses = Object.values(CharacterClass);

const {
  state: leaderboard,
  execute: loadLeaderBoard,
  isLoading: leaderBoardLoading,
} = useAsyncState(
  () => getLeaderBoard({ region: regionModel.value, characterClass: characterClassModel.value, gameMode: gameModeModel.value }),
  [],
  {}
);

watch(
  () => route.query,
  async () => {
    await loadLeaderBoard();
  }
);

const rankTable = computed(() => createRankTable());

const isSelfUser = (row: CharacterCompetitiveNumbered) => row.user.id === userStore.user!.id;

const rowClass = (row: CharacterCompetitiveNumbered) =>
  isSelfUser(row) ? 'text-primary' : 'text-content-100';
</script>

<template>
  <div class="container">
    <div class="mx-auto max-w-4xl py-8 md:py-16">
      <div class="mb-20">
        <div class="mb-5 flex justify-center">
          <OIcon icon="trophy-cup" size="5x" class="text-more-support" />
        </div>
        <Heading :title="$t('leaderboard.title')" />
      </div>
      <div class="mb-6 flex items-center justify-between gap-4">
        <OTabs v-model="regionModel" contentClass="hidden">
          <OTabItem v-for="region in regions" :label="$t(`region.${region}`, 0)" :value="region" />
        </OTabs>

        <Modal closable>
          <Tag icon="popup" variant="primary" rounded size="lg" />
          <template #popper>
            <RankTable :rankTable="rankTable" />
          </template>
        </Modal>
      </div>
      <div class="flex justify-between gap-8">
        <div class="flex items-center">
          <OTabs v-model="regionModel" contentClass="hidden" class="mb-2">
            <OTabItem v-for="region in regions" :label="$t(`region.${region}`, 0)" :value="region" />
          </OTabs>
        </div>
        <div class="flex items-right">
          <OTabs v-model="gameModeModel" contentClass="hidden" class="mb-2">
            <OTabItem v-for="gamemode in rankedGameModes" :label="$t(`game-mode.${gamemode}`, 0)" :icon="gameModeToIcon[gamemode]" :value="gamemode" />
          </OTabs>
        </div>
      </div>
      <OTable
        :data="leaderboard"
        hoverable
        bordered
        sortIcon="chevron-up"
        sortIconSize="xs"
        :loading="leaderBoardLoading"
        :rowClass="rowClass"
        :defaultSort="['position', 'asc']"
      >
        <OTableColumn
          #default="{ row }: { row: CharacterCompetitiveNumbered }"
          field="position"
          :label="$t('leaderboard.table.cols.top')"
          :width="80"
          sortable
        >
          {{ row.position }}
        </OTableColumn>

        <OTableColumn
          #default="{ row }: { row: CharacterCompetitiveNumbered }"
          field="rating.competitiveValue"
          :label="$t('leaderboard.table.cols.rank')"
          :width="230"
        >
          <Rank :rankTable="rankTable" :competitiveValue="row.rating.competitiveValue" />
        </OTableColumn>

        <OTableColumn
          #default="{ row }: { row: CharacterCompetitiveNumbered }"
          field="user.name"
          :label="$t('leaderboard.table.cols.player')"
        >
          <UserMedia :user="row.user" hiddenPlatform class="max-w-[20rem]" />
        </OTableColumn>

        <OTableColumn field="class" :width="80">
          <template #header>
            <THDropdown
              :label="$t('leaderboard.table.cols.class')"
              :shownReset="Boolean(characterClassModel)"
              @reset="characterClassModel = undefined"
            >
              <template #default="{ hide }">
                <DropdownItem
                  v-for="characterClass in characterClasses"
                  :checked="characterClass === characterClassModel"
                  @click="
                    () => {
                      characterClassModel = characterClass;
                      hide();
                    }
                  "
                >
                  <OIcon :icon="characterClassToIcon[characterClass]" size="lg" />
                  {{ $t(`character.class.${characterClass}`) }}
                </DropdownItem>
              </template>
            </THDropdown>
          </template>

          <template #default="{ row }: { row: CharacterCompetitiveNumbered }">
            <OIcon
              :icon="characterClassToIcon[row.class]"
              size="lg"
              v-tooltip="$t(`character.class.${row.class}`)"
            />
          </template>
        </OTableColumn>

        <OTableColumn
          #default="{ row }: { row: CharacterCompetitiveNumbered }"
          field="level"
          :label="$t('leaderboard.table.cols.level')"
          sortable
          :width="80"
        >
          {{ row.level }}
        </OTableColumn>

        <template #empty>
          <ResultNotFound />
        </template>
      </OTable>
    </div>
  </div>
</template>
