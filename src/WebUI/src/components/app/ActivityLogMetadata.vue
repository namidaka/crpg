<script setup lang="ts">
import { type Clan, ClanMemberRole } from '@/models/clan';
import { type UserPublic } from '@/models/user';
import { type CharacterPublic } from '@/models/character';
import { I18nT } from 'vue-i18n';
import UserClan from '@/components/user/UserClan.vue';
import ClanRole from '@/components/clan/ClanRole.vue';
import UserMedia from '@/components/user/UserMedia.vue';
import CharacterMedia from '@/components/character/CharacterMedia.vue';
import Coin from '@/components/app/Coin.vue';
import Loom from '@/components/app/Loom.vue';
import Tag from '@/components/ui/Tag.vue';

import { ActivityLog } from '@/models/activity-logs';
import { n } from '@/services/translate-service';
import { getItemImage } from '@/services/item-service';
import { Tooltip } from 'floating-vue';

const { keypath, activityLog, users, characters, clans } = defineProps<{
  keypath: string;
  activityLog: ActivityLog;
  users: UserPublic[];
  characters: CharacterPublic[];
  clans: Clan[];
}>();

const getClanById = (clanId: number) => clans.find(({ id }) => id === clanId);

const getUserById = (userId: number) => users.find(({ id }) => id === userId);

const getCharacterById = (characterId: number) => characters.find(({ id }) => id === characterId);

const emit = defineEmits<{
  read: [];
  delete: [];
}>();

const renderStrong = (value: string) => h('strong', { class: 'font-bold text-content-100' }, value);

const renderDamage = (value: string) => {
  return h('strong', { class: 'font-bold text-status-danger' }, n(Number(value)));
};

const renderUserClan = () => {
  const clan = getClanById(Number(activityLog.metadata.clanId));
  return clan
    ? h(UserClan, {
        clan,
        class: 'inline-flex items-center gap-1 align-middle',
      })
    : renderStrong(activityLog.metadata.clanId);
};

const renderClanRole = () =>
  h(ClanRole, {
    role: activityLog.metadata.oldClanMemberRole as ClanMemberRole,
  });

const renderUser = (userId: number) => {
  const user = getUserById(userId);

  // TODO:
  // <RouterLink
  //           :to="{
  //             name: 'ModeratorUserIdRestrictions',
  //             params: { id: activityLog.metadata.targetUserId },
  //           }"
  //           class="inline-block hover:text-content-100"
  //           target="_blank"
  //         >
  //           <UserMedia :user="getUserById(Number(activityLog.metadata.targetUserId))" />
  //         </RouterLink>
  //         <OButton
  //           v-if="isSelfUser"
  //           size="2xs"
  //           iconLeft="add"
  //           rounded
  //           variant="secondary"
  //           data-aq-addLogItem-addUser-btn
  //           @click="emit('addUser', Number(activityLog.metadata.targetUserId))"
  //         />

  return user
    ? h(UserMedia, {
        user,
        class: 'inline-flex items-center gap-1 align-middle font-bold text-content-100',
      })
    : renderStrong(String(userId));
};

const renderCharacter = () => {
  const character = getCharacterById(Number(activityLog.metadata.characterId));
  return character
    ? h(CharacterMedia, {
        character,
        class: 'inline-flex items-center gap-1 align-middle font-bold text-content-100',
      })
    : renderStrong(activityLog.metadata.characterId);
};

const renderItem = (itemId: string) => {
  return h(
    'span',
    {
      class: 'inline',
    },
    h(
      Tooltip,
      {
        placement: 'auto',
        class: 'inline-block',
      },
      {
        default: () => renderStrong(itemId),
        popper: () =>
          h('img', {
            src: getItemImage(itemId),
            class: 'h-full w-full object-contain',
          }),
      }
    )
  );
};

const renderGold = (value: number) => h(Coin, { value });

const renderLoom = (point: number) => h(Loom, { point });

const Render = () => {
  const { metadata } = activityLog;

  console.log('metadata.heirloomPoints', metadata.heirloomPoints);

  return h(
    I18nT,
    {
      tag: 'div',
      scope: 'global',
      keypath,
    },
    {
      clan: renderUserClan,
      oldClanMemberRole: renderClanRole,
      newClanMemberRole: renderClanRole,
      ...((activityLog.userId || metadata.userId) && {
        user: () => renderUser(Number(activityLog.userId || metadata.userId)),
      }),
      ...(metadata.actorUserId && {
        actorUser: () => renderUser(Number(metadata.actorUserId)),
      }),
      character: renderCharacter,
      ...(metadata.generation && {
        generation: () => renderStrong(metadata.generation),
      }),
      ...(metadata.level && {
        level: () => renderStrong(metadata.level),
      }),
      ...(metadata.gold && {
        gold: () => renderGold(Number(metadata.gold)),
      }),
      ...(metadata.price && {
        price: () => renderGold(Number(metadata.price)),
      }),
      ...(metadata.refundedGold && {
        refundedGold: () => renderGold(Number(metadata.refundedGold)),
      }),
      ...(metadata.heirloomPoints && {
        heirloomPoints: () => renderLoom(Number(metadata.heirloomPoints)),
      }),
      ...(metadata.refundedHeirloomPoints && {
        refundedHeirloomPoints: () => renderLoom(Number(metadata.refundedHeirloomPoints)),
      }),
      ...(metadata.itemId && {
        item: () => renderItem(metadata.itemId),
      }),
      ...(metadata.userItemId && {
        userItem: () => renderItem(metadata.userItemId),
      }),
      ...(metadata.experience && {
        experience: () => renderStrong(n(Number(metadata.experience))),
      }),
      ...(metadata.damage && {
        damage: () => renderDamage(metadata.damage),
      }),
      ...(metadata.instance && {
        instance: () => h(Tag, { variant: 'info', label: metadata.instance }),
      }),
      ...(metadata.gameMode && {
        gameMode: () => h(Tag, { variant: 'info', label: metadata.gameMode }),
      }),
      ...(metadata.oldName && {
        oldName: () => renderStrong(metadata.oldName),
      }),
      ...(metadata.newName && {
        newName: () => renderStrong(metadata.newName),
      }),
      ...(metadata.message && {
        message: () => renderStrong(metadata.message),
      }),
    }
  );
};
</script>

<template>
  <Render />
</template>
