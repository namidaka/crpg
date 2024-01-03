<script setup lang="ts">
import { moderationUserKey } from '@/symbols/moderator';
import { getCharactersByUserId } from '@/services/characters-service';
import { updateUserNote } from '@/services/users-service';
import { notify } from '@/services/notification-service';

definePage({
  props: true,
  meta: {
    layout: 'default',
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
