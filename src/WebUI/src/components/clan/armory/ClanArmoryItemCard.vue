<script setup lang="ts">
import { type ClanArmoryItem } from '@/models/clan';
import { type UserPublic } from '@/models/user';
import { useUserStore } from '@/stores/user';

const { clanArmoryItem } = defineProps<{
  clanArmoryItem: ClanArmoryItem;
  // TODO:
  owner: UserPublic;
  borrower: UserPublic | null;
}>();

const emit = defineEmits<{}>();

const { user } = toRefs(useUserStore());
</script>

<template>
  <ItemCard class="cursor-pointer" :item="clanArmoryItem.userItem.item">
    <template #badges-top-right>
      <VTooltip>
        <UserMedia :user="owner" hiddenPlatform hiddenTitle :isSelf="user!.id === owner.id" />
        <template #popper>
          <div class="flex items-center gap-2">
            <i18n-t
              scope="global"
              keypath="clan.armory.item.owner.tooltip.title"
              tag="div"
              class="flex items-center gap-2"
            >
              <template #user>
                <UserMedia
                  class="max-w-[10rem]"
                  :user="owner"
                  :isSelf="user!.id === clanArmoryItem.userItem.userId"
                  hiddenPlatform
                />
              </template>
            </i18n-t>
          </div>
        </template>
      </VTooltip>

      <VTooltip v-if="borrower">
        <UserMedia
          :user="borrower"
          hiddenPlatform
          hiddenTitle
          class="-ml-4"
          :isSelf="user!.id === borrower.id"
        />
        <template #popper>
          <div class="flex items-center gap-2">
            <i18n-t
              scope="global"
              keypath="clan.armory.item.borrower.tooltip.title"
              tag="div"
              class="flex items-center gap-2"
            >
              <template #user>
                <UserMedia
                  class="max-w-[10rem]"
                  :user="borrower"
                  :isSelf="user!.id === borrower.id"
                  hiddenPlatform
                />
              </template>
            </i18n-t>
          </div>
        </template>
      </VTooltip>
    </template>
  </ItemCard>
</template>
