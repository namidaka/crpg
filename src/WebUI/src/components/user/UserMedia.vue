<script setup lang="ts">
import { ClanMemberRole } from '@/models/clan';
import { type UserPublic } from '@/models/user';

const {
  user,
  clanRole = null,
  isSelf = false,
  hiddenPlatform = false,
  hiddenTitle = false,
  hiddenClan = false,
  size = 'sm',
} = defineProps<{
  user: UserPublic;
  clanRole?: ClanMemberRole | null;
  isSelf?: boolean;
  hiddenPlatform?: boolean;
  hiddenTitle?: boolean;
  hiddenClan?: boolean;
  size?: 'sm' | 'xl';
}>();
</script>

<template>
  <div class="flex items-center gap-1.5">
    <img
      :src="user.avatar"
      class="rounded-full"
      :alt="user.name"
      :class="[size === 'xl' ? 'h-8 w-8' : 'h-6 w-6', { 'ring-2  ring-status-success': isSelf }]"
    />

    <UserClan v-if="!hiddenClan && user.clan" :clan="user.clan" :clanRole="clanRole" />

    <div
      v-if="!hiddenTitle"
      class="max-w-full overflow-hidden overflow-ellipsis whitespace-nowrap"
      :title="user.name"
    >
      {{ user.name }}
      <template v-if="isSelf">({{ $t('you') }})</template>
    </div>

    <UserPlatform
      v-if="!hiddenPlatform"
      :platform="user.platform"
      :platformUserId="user.platformUserId"
      :userName="user.name"
      :size="size"
    />
  </div>
</template>
