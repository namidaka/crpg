<script setup lang="ts">
import { ClanMemberRole } from '@/models/clan';
import { type Clan } from '@/models/clan';

const { clan, clanRole } = defineProps<{
  clan: Clan;
  clanRole?: ClanMemberRole | null;
}>();
</script>

<template>
  <RouterLink
    class="group flex items-center gap-1 hover:opacity-75"
    :to="{ name: 'ClansId', params: { id: clan.id } }"
  >
    <ClanRoleIcon
      v-if="clanRole && [ClanMemberRole.Leader, ClanMemberRole.Officer].includes(clanRole)"
      :role="clanRole"
    />
    <ClanTagIcon :color="clan.primaryColor" />
    [{{ clan.tag }}]
  </RouterLink>
</template>
