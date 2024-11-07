<script setup lang="ts">
import type { UserItem, UserPublic } from '~/models/user'

import { getItemGraceTimeEnd, isGraceTimeExpired } from '~/services/item-service'
import { useUserStore } from '~/stores/user'

const {
  equipped = false,
  notMeetRequirement = false,
  userItem,
} = defineProps<{
  userItem: UserItem
  equipped: boolean
  notMeetRequirement: boolean
  lender?: UserPublic | null
}>()

const { user } = toRefs(useUserStore())

const isNew = computed(() => !isGraceTimeExpired(getItemGraceTimeEnd(userItem)))
</script>

<template>
  <ItemCard
    :item="userItem.item"
    :class="{ 'bg-primary-hover/15 ': userItem.isPersonal }"
  >
    <template #badges-top-right>
      <Tag
        v-if="userItem.isBroken"
        v-tooltip="$t('character.inventory.item.broken.tooltip.title')"
        rounded
        variant="danger"
        icon="error"
        class="cursor-default opacity-80 hover:opacity-100"
      />

      <template v-if="userItem.isArmoryItem">
        <ClanArmoryItemRelationBadge
          v-if="lender && lender.id !== user!.id"
          :lender="lender"
        />
        <Tag
          v-else
          v-tooltip="$t('character.inventory.item.clanArmory.inArmory.title')"
          rounded
          size="lg"
          variant="primary"
          icon="armory"
          class="cursor-default opacity-80 hover:opacity-100"
        />
      </template>
    </template>

    <template #badges-bottom-left>
      <Tag
        v-if="isNew"
        variant="success"
        label="new"
      />
    </template>

    <template #badges-bottom-right>
      <Tag
        v-if="notMeetRequirement"
        v-tooltip="$t('character.inventory.item.requirement.tooltip.title')"
        rounded
        variant="danger"
        icon="alert"
      />

      <Tag
        v-if="equipped"
        v-tooltip="$t('character.inventory.item.equipped')"
        rounded
        variant="success"
        icon="check"
      />
    </template>
  </ItemCard>
</template>
