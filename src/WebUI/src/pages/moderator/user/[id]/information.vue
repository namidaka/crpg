<script setup lang="ts">
import { moderationUserKey } from '@/symbols/moderator';
import { getCharactersByUserId } from '@/services/characters-service';
import { updateUserNote } from '@/services/users-service';
import { notify } from '@/services/notification-service';

definePage({
  props: true,
  meta: {
    roles: ['Moderator', 'Admin'],
  },
});

defineProps<{ id: string }>();
const emit = defineEmits<{ update: [] }>();
const user = injectStrict(moderationUserKey);

const note = ref<string>(user.value?.note || '');

const { state: characters } = await useAsyncState(() => getCharactersByUserId(user.value!.id), []);

const onSubmitNoteForm = async () => {
  if (user.value!.note !== note.value) {
    await updateUserNote(user.value!.id, { note: note.value });
    notify('The user note has been updated');
    emit('update');
  }
};

interface RewardForm {
  gold: number;
  heirloomPoints: number;
  characterId?: number;
  experience: number;
}

const rewardFormModel = ref<RewardForm>({
  gold: 0,
  heirloomPoints: 0,
  characterId: characters.value[0].id,
  experience: 0,
});

const selectedCharacter = computed(() =>
  characters.value.find(c => c.id === rewardFormModel.value.characterId)
);
</script>

<template>
  <div class="mx-auto max-w-3xl space-y-8 pb-8">
    <FormGroup v-if="user" :collapsable="false">
      <div class="grid grid-cols-2 gap-2 text-2xs">
        <SimpleTableRow :label="'Id'">
          {{ user.id }}
        </SimpleTableRow>
        <SimpleTableRow :label="'Region'">
          {{ $t(`region.${user.region}`, 0) }}
        </SimpleTableRow>
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
        <SimpleTableRow :label="'Created'">
          {{ $d(user.createdAt, 'long') }}
        </SimpleTableRow>
        <SimpleTableRow :label="'Last activity'">
          {{ $d(user.updatedAt, 'long') }}
        </SimpleTableRow>
        <SimpleTableRow :label="'Gold'">
          <Coin :value="user.gold" />
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

    <FormGroup :collapsable="false">
      <template #label>
        <SvgSpriteImg name="coin" viewBox="0 0 18 18" class="w-5" />
        Rewards
      </template>
      <form @submit.prevent="onSubmitNoteForm" class="space-y-8">
        <div class="grid grid-cols-2 gap-4">
          <OField label="Gold">
            <OInput
              placeholder="Gold"
              v-model="rewardFormModel.gold"
              size="lg"
              type="number"
              expanded
            >
              <template #le></template>
            </OInput>
          </OField>

          <OField label="Heirloom points">
            <OInput
              placeholder="Heirloom points"
              v-model="rewardFormModel.heirloomPoints"
              size="lg"
              expanded
            />
          </OField>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <OField class="col-span-2">
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
                  <DropdownItem v-for="character in characters">
                    <CharacterMedia
                      :character="character"
                      :isActive="character.id === user?.activeCharacterId"
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

          <OField class="col-span-1" label="Experience">
            <OInput
              placeholder="Experience"
              v-model="rewardFormModel.experience"
              size="lg"
              expanded
            />
          </OField>
        </div>

        <OButton native-type="submit" variant="primary" size="lg" :label="`Submit`" />
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
