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

const user = injectStrict(moderationUserKey);

const { state: characters } = useAsyncState(() => getCharactersByUserId(user.value!.id), []);
</script>

<template>
  <div class="mx-auto max-w-3xl space-y-8 pb-8">
    <div class="space-y-3">
      <div>Id: {{ user!.id }}</div>
      <div v-if="user?.clan" class="flex items-center gap-1">
        Clan: {{ user.clan.name }}
        <UserClan :clan="user.clan" />
      </div>
      <div>Created: {{ $d(user!.createdAt, 'long') }}</div>
      <div>Last activity: {{ $d(user!.updatedAt, 'long') }}</div>
      <div class="flex items-center gap-1">
        Platform: {{ user!.platform }} {{ user!.platformUserId }}
        <UserPlatform
          :platform="user!.platform"
          :platformUserId="user!.platformUserId"
          :userName="user!.name"
        />
      </div>
    </div>

    <div class="space-y-2">
      <div>Characters:</div>
      <div class="flex gap-2">
        <CharacterMedia v-for="character in characters" :character="character" />
      </div>
    </div>
  </div>
</template>
