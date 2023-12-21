<script setup lang="ts">
import { type ClanArmoryItem } from '@/models/clan';
import { type UserPublic } from '@/models/user';
import { useUserStore } from '@/stores/user';
import { isOwnClanArmoryItem, isClanArmoryItemInInventory } from '@/services/clan-service';

const { clanArmoryItem } = defineProps<{
  clanArmoryItem: ClanArmoryItem;
  lender: UserPublic;
  borrower: UserPublic | null;
}>();

const emit = defineEmits<{
  borrow: [id: number];
  remove: [id: number];
  return: [id: number];
}>();

const { user, userItems } = toRefs(useUserStore());

const isOwnArmoryItem = computed(() => isOwnClanArmoryItem(clanArmoryItem, user.value!.id));
const isInInventory = computed(() => isClanArmoryItemInInventory(clanArmoryItem, userItems.value));
</script>

<template>
  <ItemDetail :item="clanArmoryItem.userItem.item">
    <template #badges-bottom-right>
      <ClanArmoryItemRelationBadge :lender="lender" :borrower="borrower" />
    </template>

    <template #actions>
      <ConfirmActionTooltip
        v-if="isOwnArmoryItem"
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
          icon-left="armory"
          :label="$t('clan.armory.item.remove.title')"
        />
      </ConfirmActionTooltip>

      <div v-else-if="!borrower" class="flex w-full flex-col items-center justify-center gap-2">
        <OButton
          variant="secondary"
          expanded
          rounded
          :disabled="isInInventory"
          size="lg"
          v-tooltip="{
            disabled: !isInInventory,
            content: $t('clan.armory.item.borrow.validation.isInInventory'),
            popperClass: 'v-popper--theme-tooltip-danger',
          }"
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
      </div>

      <template v-else>
        <OButton
          v-if="borrower.id === user!.id"
          variant="secondary"
          icon-left="armory"
          expanded
          rounded
          size="lg"
          :label="$t('clan.armory.item.return.title')"
          @click="$emit('return', clanArmoryItem.userItem.id)"
        />

        <OButton
          v-else
          variant="secondary"
          disabled
          expanded
          rounded
          size="lg"
          v-tooltip="{
            content: $t('clan.armory.item.borrow.validation.borrowed'),
            popperClass: 'v-popper--theme-tooltip-danger',
          }"
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
      </template>
    </template>
  </ItemDetail>
</template>
