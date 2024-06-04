<script setup lang="ts">
import { moderationUserKey } from '@/symbols/moderator';
import {
  getCharactersByUserId,
  rewardCharacter,
  getLevelByExperience,
  getAutoRetireCount,
  getExperienceMultiplierBonusByRetireCount,
  sumExperienceMultiplierBonus,
} from '@/services/characters-service';
import { updateUserNote, rewardUser } from '@/services/users-service';
import { notify } from '@/services/notification-service';
import { n } from '@/services/translate-service';
import { useUserStore } from '@/stores/user';
import Role from '@/models/role';

definePage({
  props: true,
  meta: {
    roles: ['Moderator', 'Admin'],
  },
});

defineProps<{ id: string }>();

const userStore = useUserStore();

const emit = defineEmits<{ update: [] }>();
const user = injectStrict(moderationUserKey);

const note = ref<string>(user.value?.note || '');

const { state: characters, execute: loadCharacters } = await useAsyncState(
  () => getCharactersByUserId(user.value!.id),
  [],
  {
    resetOnExecute: false,
  }
);

const onSubmitNoteForm = async () => {
  if (user.value!.note !== note.value) {
    await updateUserNote(user.value!.id, { note: note.value });
    notify('The user note has been updated');
    emit('update');
  }
};

// TODO: Reward - refactoring, spec
const canReward = computed(() => userStore.user!.role === Role.Admin); // TODO: to service

interface RewardForm {
  gold: number;
  heirloomPoints: number;
  characterId?: number;
  autoRetire: boolean;
  experience: number;
}

const defaultRewardForm = computed<RewardForm>(() => ({
  gold: 0,
  heirloomPoints: 0,
  characterId: characters.value[0].id,
  autoRetire: false,
  experience: 0,
}));

const rewardFormModel = ref<RewardForm>({ ...defaultRewardForm.value });

const selectedCharacter = computed(() =>
  characters.value.find(c => c.id === rewardFormModel.value.characterId)
);

// TODO: to cmp, or composable
const experienceModel = computed({
  get() {
    return n(rewardFormModel.value.experience);
  },
  set(_val: string) {
    if (_val === '-') {
      _val = '-0';
    }
    const val = Number(_val.replace(/[^.0-9\\-]/g, ''));
    rewardFormModel.value.experience = val;
  },
});

// TODO: to cmp, or composable
const goldModel = computed({
  get() {
    return n(rewardFormModel.value.gold);
  },
  set(_val: string) {
    if (_val === '-') {
      _val = '-0';
    }
    const val = Number(_val.replace(/[^.0-9\\-]/g, ''));
    rewardFormModel.value.gold = val;
  },
});

const onSubmitRewardForm = async () => {
  if (!canReward.value) return;

  if (rewardFormModel.value.gold !== 0 || rewardFormModel.value.heirloomPoints !== 0) {
    await rewardUser(user.value!.id, {
      gold: rewardFormModel.value.gold,
      heirloomPoints: rewardFormModel.value.heirloomPoints,
    });
    notify('The user has been rewarded');
  }

  if (rewardFormModel.value.characterId && rewardFormModel.value.experience !== 0) {
    await rewardCharacter(user.value!.id, rewardFormModel.value.characterId!, {
      experience: rewardFormModel.value.experience,
      autoRetire: rewardFormModel.value.autoRetire,
    });
    notify('The character has been rewarded');
  }

  rewardFormModel.value = {
    ...defaultRewardForm.value,
    characterId: rewardFormModel.value.characterId,
  };

  await loadCharacters();
  emit('update');
};

const totalRewardValues = computed(() => {
  const gold = user.value!.gold + rewardFormModel.value.gold;
  const heirloomPoints = user.value!.heirloomPoints + rewardFormModel.value.heirloomPoints;

  if (rewardFormModel.value.autoRetire) {
    const { retireCount, remainExperience } = getAutoRetireCount(
      rewardFormModel.value.experience,
      selectedCharacter.value!.experience
    );

    return {
      gold,
      heirloomPoints,
      experience: remainExperience,
      level: getLevelByExperience(remainExperience),
      experienceMultiplier: sumExperienceMultiplierBonus(
        user.value!.experienceMultiplier,
        getExperienceMultiplierBonusByRetireCount(retireCount)
      ),
    };
  }

  return {
    gold,
    heirloomPoints,
    experience: selectedCharacter.value!.experience + rewardFormModel.value.experience,
    level: getLevelByExperience(
      selectedCharacter.value!.experience + rewardFormModel.value.experience
    ),
    experienceMultiplier: user.value!.experienceMultiplier,
  };
});
</script>

<template>
  <div class="mx-auto max-w-3xl space-y-8 pb-8">
    <FormGroup v-if="user" label="User" :collapsable="false">
      <div class="grid grid-cols-2 gap-2 text-2xs">
        <SimpleTableRow :label="'Id'" :value="String(user.id)" />
        <SimpleTableRow
          :label="$t('character.statistics.expMultiplier.title')"
          :value="
            $t('character.format.expMultiplier', {
              multiplier: $n(user.experienceMultiplier),
            })
          "
        />
        <SimpleTableRow :label="'Region'" :value="$t(`region.${user.region}`, 0)" />
        <SimpleTableRow :label="'Platform'">
          {{ user.platform }} {{ user.platformUserId }}
          <UserPlatform
            :platform="user.platform"
            :platformUserId="user.platformUserId"
            :userName="user.name"
          />
        </SimpleTableRow>
        <SimpleTableRow v-if="user?.clan" :label="'Clan'">
          {{ user.clan.name }}
          <UserClan :clan="user.clan" />
        </SimpleTableRow>
        <SimpleTableRow :label="'Created'" :value="$d(user.createdAt, 'long')" />
        <SimpleTableRow :label="'Last activity'" :value="$d(user.updatedAt, 'long')" />
        <SimpleTableRow :label="'Gold'">
          <Coin :value="user.gold" />
        </SimpleTableRow>
        <SimpleTableRow :label="'Heirloom'">
          <Heirloom :value="user.heirloomPoints" />
        </SimpleTableRow>
      </div>
    </FormGroup>

    <FormGroup :label="'Characters'" :collapsable="false">
      <div class="flex flex-wrap gap-3">
        <CharacterMedia
          class="rounded-full border border-border-200 px-3 py-2"
          v-for="character in characters"
          :character="character"
          :isActive="character.id === user?.activeCharacterId"
        />
      </div>
    </FormGroup>

    <FormGroup v-if="canReward" :collapsable="false" label="Rewards" canReward>
      <form @submit.prevent class="space-y-8">
        <div class="grid grid-cols-2 gap-4">
          <OField>
            <template #label>
              <div class="flex items-center gap-1.5">
                <SvgSpriteImg name="coin" viewBox="0 0 18 18" class="w-4.5" />
                Gold
              </div>
            </template>
            <OInput placeholder="Gold" v-model="goldModel" size="lg" expanded />
          </OField>

          <OField>
            <template #label>
              <div class="flex items-center gap-1.5">
                <OIcon icon="blacksmith" size="sm" class="text-primary" />
                Heirloom points
              </div>
            </template>

            <OInput
              placeholder="Heirloom points"
              v-model="rewardFormModel.heirloomPoints"
              size="lg"
              type="number"
              expanded
            />
          </OField>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <OField class="col-span-2" label="Character">
            <VDropdown :triggers="['click']">
              <template #default="{ shown }">
                <OButton variant="secondary" outlined size="lg">
                  <CharacterMedia :character="selectedCharacter!" />
                  <Divider inline />
                  <OIcon
                    icon="chevron-down"
                    size="lg"
                    :rotation="shown ? 180 : 0"
                    class="text-content-400"
                  />
                </OButton>
              </template>

              <template #popper="{ hide }">
                <div class="max-h-64 max-w-md overflow-y-auto">
                  <DropdownItem
                    v-for="character in characters"
                    :active="character.id === selectedCharacter!.id"
                  >
                    <CharacterMedia
                      :character="character"
                      @click="
                        () => {
                          rewardFormModel.characterId = character.id;
                          hide();
                        }
                      "
                    />
                  </DropdownItem>
                </div>
              </template>
            </VDropdown>
          </OField>

          <OField class="col-span-1">
            <template #label>
              <div class="flex items-center gap-1.5">
                <OIcon icon="experience" size="lg" class="text-primary" />
                Experience
              </div>
            </template>
            <OInput placeholder="Experience" v-model="experienceModel" size="lg" expanded />
          </OField>

          <OField class="col-span-1" label="Auto retire">
            <OSwitch v-model="rewardFormModel.autoRetire" />
          </OField>

          <div class="col-span-2 space-y-4">
            <Divider />

            <div v-if="rewardFormModel.heirloomPoints" class="flex items-center gap-2 font-bold">
              <OIcon icon="blacksmith" size="lg" class="text-primary" />
              {{ $n(user!.heirloomPoints) }}
              <span>-></span>
              <span
                :class="[
                  rewardFormModel.heirloomPoints < 0 ? 'text-status-danger' : 'text-status-success',
                ]"
              >
                {{ $n(totalRewardValues.heirloomPoints) }}
              </span>
            </div>

            <div v-if="rewardFormModel.gold" class="flex items-center gap-2 font-bold">
              <SvgSpriteImg name="coin" viewBox="0 0 18 18" class="w-4.5" />
              {{ $n(user!.gold) }}
              <span>-></span>
              <span
                :class="[rewardFormModel.gold < 0 ? 'text-status-danger' : 'text-status-success']"
              >
                {{ $n(totalRewardValues.gold) }}
              </span>
            </div>

            <template v-if="rewardFormModel.experience">
              <div class="flex items-center gap-2 font-bold">
                <OIcon icon="experience" size="lg" class="text-primary" />
                {{ $n(selectedCharacter!.experience) }}
                <span>-></span>
                <span
                  :class="[
                    rewardFormModel.experience < 0 ? 'text-status-danger' : 'text-status-success',
                  ]"
                >
                  {{ $n(totalRewardValues.experience) }}
                </span>
              </div>

              <div
                v-if="
                  rewardFormModel.autoRetire &&
                  totalRewardValues.experienceMultiplier - user!.experienceMultiplier !== 0
                "
                class="flex items-center gap-2 font-bold"
              >
                <span>exp. multi</span>
                {{ $n(user!.experienceMultiplier) }}
                <span>-></span>
                <span class="text-status-success">
                  {{ $n(totalRewardValues.experienceMultiplier) }}
                </span>
              </div>

              <div
                v-if="totalRewardValues.level - selectedCharacter!.level !== 0"
                class="flex items-center gap-2 font-bold"
              >
                <span>lvl</span>
                <span>{{ selectedCharacter!.level }}</span>
                <span>-></span>
                <span
                  class="text-status-success"
                  :class="[
                    rewardFormModel.experience < 0 ? 'text-status-danger' : 'text-status-success',
                  ]"
                >
                  {{ totalRewardValues.level }}
                </span>
              </div>
            </template>
          </div>
        </div>

        <div>
          <ConfirmActionTooltip
            :confirmLabel="$t('action.ok')"
            :title="`Are you sure you want to reward this user?`"
            placement="bottom"
            @confirm="onSubmitRewardForm"
          >
            <OButton native-type="submit" variant="primary" size="lg" :label="`Submit`" />
          </ConfirmActionTooltip>
        </div>
      </form>
    </FormGroup>

    <FormGroup :label="'Note'" :collapsable="false">
      <form @submit.prevent="onSubmitNoteForm" class="space-y-8">
        <OField :message="'For internal use'">
          <OInput
            placeholder="User note"
            v-model="note"
            size="lg"
            expanded
            type="textarea"
            rows="6"
          />
        </OField>

        <OButton
          native-type="submit"
          :disabled="user!.note === note"
          variant="primary"
          size="lg"
          :label="`Update`"
        />
      </form>
    </FormGroup>
  </div>
</template>
