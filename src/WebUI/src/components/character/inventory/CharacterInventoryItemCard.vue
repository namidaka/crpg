<script setup lang="ts">
import { type UserItem, type UserPublic } from '@/models/user';
import { getItemGraceTimeEnd, isGraceTimeExpired } from '@/services/item-service';
import { useUserStore } from '@/stores/user';

const {
  userItem,
  equipped = false,
  notMeetRequirement = false,
} = defineProps<{
  userItem: UserItem;
  equipped: boolean;
  notMeetRequirement: boolean;
  lender?: UserPublic | null;
}>();

const { user } = toRefs(useUserStore());

const isNew = computed(() => !isGraceTimeExpired(getItemGraceTimeEnd(userItem)));
</script>

<template>
  <ItemCard :item="userItem.item" :class="{ 'bg-primary-hover/15 ': userItem.isPersonal }">
    <template #badges-top-right>
      <Tag
        v-if="userItem.isBroken"
        rounded
        variant="danger"
        icon="error"
        v-tooltip="$t('character.inventory.item.broken.tooltip.title')"
        class="cursor-default opacity-80 hover:opacity-100"
      />

      <template v-if="userItem.isArmoryItem">
        <ClanArmoryItemRelationBadge v-if="lender && lender.id !== user!.id" :lender="lender" />
        <Tag
          v-else
          rounded
          size="lg"
          variant="primary"
          icon="armory"
          class="cursor-default opacity-80 hover:opacity-100"
          v-tooltip="$t('character.inventory.item.clanArmory.inArmory.title')"
        />
      </template>
    </template>

    <template #badges-bottom-left>
      <Tag v-if="isNew" variant="success" label="new" />
    </template>

    <template #badges-bottom-right>
      <Tag
        v-if="notMeetRequirement"
        rounded
        variant="danger"
        icon="alert"
        v-tooltip="$t('character.inventory.item.requirement.tooltip.title')"
      />

      <Tag
        v-if="equipped"
        rounded
        variant="success"
        icon="check"
        v-tooltip="$t('character.inventory.item.equipped')"
      />
    </template>
  </ItemCard>
</template>
