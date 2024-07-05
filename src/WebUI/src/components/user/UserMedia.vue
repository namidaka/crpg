<script setup lang="ts">
import { ClanMemberRole, type Clan } from '@/models/clan';
import { type UserPublic, type UserClan } from '@/models/user';

const {
  user,
  clanRole = null,
  clan = null,
  isSelf = false,
  hiddenPlatform = false,
  hiddenTitle = false,
  hiddenClan = false,
  size = 'sm',
} = defineProps<{
  user: UserPublic;
  clanRole?: ClanMemberRole | null;
  clan?: Clan | null;
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
      :class="[size === 'xl' ? 'h-8 w-8' : 'h-6 w-6', { 'ring-2 ring-status-success': isSelf }]"
    />

    <UserClan v-if="!hiddenClan && user.clan" :clan="user.clan" :clanRole="clanRole" />
    <template v-if="!hiddenClan && clan">
      <RouterLink
        class="group flex items-center gap-1 hover:opacity-75"
        :to="{ name: 'ClansId', params: { id: clan.id } }"
      >
        <ClanRoleIcon
          v-if="
            clanRole !== null && [ClanMemberRole.Leader, ClanMemberRole.Officer].includes(clanRole)
          "
          :role="clanRole"
        />
        <ClanTagIcon :color="clan.primaryColor" />
        [{{ clan.tag }}]
      </RouterLink>
    </template>

    <div
      v-if="!hiddenTitle"
      :title="user.name"
      class="max-w-full overflow-hidden overflow-ellipsis whitespace-nowrap"
    >
      {{ user.name }}
    </div>

    <div v-if="isSelf" class="text-3xs text-content-300">({{ $t('you') }})</div>

    <UserPlatform
      v-if="!hiddenPlatform"
      :platform="user.platform"
      :platformUserId="user.platformUserId"
      :userName="user.name"
      :size="size"
    />
  </div>
</template>
