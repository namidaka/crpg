<script setup lang="ts">
import { type CharacterCompetitiveNumbered } from '@/models/competitive';
import { getLeaderBoard, createRankTable } from '@/services/leaderboard-service';
import { characterClassToIcon } from '@/services/characters-service';
import { useUserStore } from '@/stores/user';
import { useRegion } from '@/composables/use-region';

definePage({
  meta: {
    layout: 'default',
    bg: 'background-2.webp',
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const userStore = useUserStore();

const { regionModel, regions } = useRegion();

const {
  state: leaderboard,
  execute: loadLeaderBoard,
  isLoading: leaderBoardLoading,
} = useAsyncState(() => getLeaderBoard(regionModel.value), [], {});

watch(regionModel, () => {
  loadLeaderBoard();
});

const rankTable = computed(() => createRankTable());

const isSelfUser = (row: CharacterCompetitiveNumbered) => row.user.id === userStore.user!.id;

const rowClass = (row: CharacterCompetitiveNumbered) =>
  isSelfUser(row) ? 'text-primary' : 'text-content-100';
</script>

<template>
  <div class="container">
    <div class="mx-auto max-w-4xl py-8 md:py-16">
      <div class="container mb-20">
        <div class="mb-5 flex justify-center">
          <OIcon icon="trophy-cup" size="5x" class="text-more-support" />
        </div>

        <div class="item-center flex select-none justify-center gap-4 md:gap-8">
          <SvgSpriteImg
            name="logo-decor"
            viewBox="0 0 108 10"
            class="w-16 rotate-180 transform md:w-28"
          />
          <h1 class="text-2xl text-content-100">{{ $t('leaderboard.title') }}</h1>

          <SvgSpriteImg name="logo-decor" viewBox="0 0 108 10" class="w-16 md:w-28" />
        </div>
      </div>

      <div class="flex items-center justify-between gap-4">
        <OTabs v-model="regionModel" contentClass="hidden" class="mb-6">
          <OTabItem v-for="region in regions" :label="$t(`region.${region}`, 0)" :value="region" />
        </OTabs>

        <Modal closable>
          <Tag icon="popup" variant="primary" rounded size="lg" />
          <template #popper>
            <RankTable :rankTable="rankTable" />
          </template>
        </Modal>
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
          :width="120"
          sortable
        >
          {{ row.position }}
          <span v-if="isSelfUser(row)">({{ $t('you') }})</span>
        </OTableColumn>

        <OTableColumn
          #default="{ row }: { row: CharacterCompetitiveNumbered }"
          field="rating.competitiveValue"
          :label="$t('leaderboard.table.cols.rank')"
          :width="220"
        >
          <Rank :rankTable="rankTable" :competitiveValue="row.rating.competitiveValue" />
        </OTableColumn>

        <OTableColumn
          #default="{ row }: { row: CharacterCompetitiveNumbered }"
          field="user.name"
          :label="$t('leaderboard.table.cols.player')"
          :width="180"
        >
          <UserMedia :user="row.user" :clan="row.user.clan" hiddenPlatform />
        </OTableColumn>

        <OTableColumn
          #default="{ row }: { row: CharacterCompetitiveNumbered }"
          field="class"
          :label="$t('leaderboard.table.cols.class')"
          sortable
        >
          <OIcon
            :icon="characterClassToIcon[row.class]"
            size="lg"
            v-tooltip="$t(`character.class.${row.class}`)"
          />
        </OTableColumn>

        <OTableColumn
          #default="{ row }: { row: CharacterCompetitiveNumbered }"
          field="level"
          :label="$t('leaderboard.table.cols.level')"
        >
          {{ row.level }}
        </OTableColumn>

        <OTableColumn
          #default="{ row }: { row: CharacterCompetitiveNumbered }"
          field="user.region"
          :label="$t('leaderboard.table.cols.region')"
        >
          {{ $t(`region.${row.user.region}`, 0) }}
        </OTableColumn>

        <template #empty>
          <ResultNotFound />
        </template>
      </OTable>
    </div>
  </div>
</template>