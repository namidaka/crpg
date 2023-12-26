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
      :class="[size === 'xl' ? 'h-9 w-9' : 'h-6 w-6', { 'ring-2  ring-status-success': isSelf }]"
    />

    <template v-if="!hiddenClan && user.clan">
      <RouterLink
        class="group flex items-center gap-1 hover:opacity-75"
        :to="{ name: 'ClansId', params: { id: user.clan.id } }"
      >
        <ClanRoleIcon
          v-if="
            clanRole !== null && [ClanMemberRole.Leader, ClanMemberRole.Officer].includes(clanRole)
          "
          :role="clanRole"
        />
        <ClanTagIcon :color="user.clan.primaryColor" />
        [{{ user.clan.tag }}]
      </RouterLink>
    </template>

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
