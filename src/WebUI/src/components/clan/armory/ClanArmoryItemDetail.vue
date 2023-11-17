<script setup lang="ts">
import { type ClanArmoryItem } from '@/models/clan';
import { type UserPublic } from '@/models/user';
import { useUserStore } from '@/stores/user';

const { clanArmoryItem } = defineProps<{
  clanArmoryItem: ClanArmoryItem;
  lender: UserPublic;
  borrower: UserPublic | null;
}>();

const emit = defineEmits<{
  borrow: [id: number];
  remove: [id: number];
}>();

const { user } = toRefs(useUserStore());

const isOwnItem = computed(() => user.value?.id === clanArmoryItem.userItem.userId);
</script>

<template>
  <ItemDetail :item="clanArmoryItem.userItem.item">
    <template #actions>
      <ConfirmActionTooltip
        v-if="isOwnItem"
        class="flex-auto"
        :confirmLabel="$t('action.ok')"
        :title="$t('clan.armory.item.remove.confirm.description')"
        @confirm="$emit('remove', clanArmoryItem.userItem.id)"
      >
        <OButton
          variant="warning"
          expanded
          rounded
          size="lg"
          :label="$t('clan.armory.item.remove.title')"
        />
      </ConfirmActionTooltip>

      <OButton
        v-else-if="borrower"
        variant="secondary"
        disabled
        expanded
        rounded
        size="lg"
        v-tooltip="$t('clan.armory.item.borrow.validation.borrowed')"
      >
        <i18n-t
          scope="global"
          keypath="clan.armory.item.borrowed.title"
          tag="div"
          class="flex items-center gap-2"
        >
          <template #user>
            <UserMedia :user="borrower" hiddenPlatform hiddenClan />
          </template>
        </i18n-t>
      </OButton>

      <OButton
        v-else
        variant="secondary"
        expanded
        rounded
        size="lg"
        @click="$emit('borrow', clanArmoryItem.userItem.id)"
      >
        <i18n-t
          scope="global"
          keypath="clan.armory.item.borrow.title"
          tag="div"
          class="flex items-center gap-2"
        >
          <template #user>
            <UserMedia :user="lender" hiddenPlatform hiddenClan />
          </template>
        </i18n-t>
      </OButton>
    </template>
  </ItemDetail>
</template>
