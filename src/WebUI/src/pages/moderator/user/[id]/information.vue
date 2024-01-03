<script setup lang="ts">
import { moderationUserKey } from '@/symbols/moderator';
import { getCharactersByUserId } from '@/services/characters-service';

definePage({
  props: true,
  meta: {
    layout: 'default',
    roles: ['Moderator', 'Admin'],
  },
});

defineProps<{ id: string }>();

const note = ref<string>(
  'Lorem ipsum dolor sit amet consectetur adipisicing elit. Autem modi quae necessitatibus excepturi voluptatum repellendus libero iure aliquam accusamus, ea quaerat recusandae architecto perspiciatis eos. Laudantium cupiditate magnam rem rerum.'
);
const user = injectStrict(moderationUserKey);

const { state: characters } = await useAsyncState(() => getCharactersByUserId(user.value!.id), []);
</script>

<template>
  <div class="mx-auto max-w-3xl space-y-8 pb-8">
    <FormGroup v-if="user" :label="'Main'" :collapsable="false">
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
      <div class="flex gap-2">
        <CharacterMedia v-for="character in characters" :character="character" />
      </div>
    </FormGroup>

    <FormGroup :label="'Note'" :collapsable="false">
      <OField>
        <OInput
          placeholder="User note"
          v-model="note"
          size="lg"
          expanded
          required
          type="textarea"
          rows="6"
        />
      </OField>

      <div>
        <OButton native-type="submit" variant="primary" size="lg" :label="`Update`" />
      </div>
    </FormGroup>
  </div>
</template>
