<script setup lang="ts">
import { type UserItem, type UserPublic } from '@/models/user';
import { getItemGraceTimeEnd, isGraceTimeExpired } from '@/services/item-service';

const {
  userItem,
  equipped = false,
  notMeetRequirement = false,
} = defineProps<{
  userItem: UserItem;
  equipped: boolean;
  notMeetRequirement: boolean;
  owner?: UserPublic | null;
}>();

const isNew = computed(() => !isGraceTimeExpired(getItemGraceTimeEnd(userItem)));
</script>

<template>
  <ItemCard :item="userItem.item">
    <template #badges-top-right>
      <Tag
        v-if="userItem.isBroken"
        rounded
        variant="danger"
        icon="error"
        v-tooltip="$t('character.inventory.item.broken.tooltip.title')"
        class="cursor-default opacity-80 hover:opacity-100"
      />

      <CharacterInventoryItemArmoryTag v-if="owner || userItem.isArmoryItem" :owner="owner" />
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
