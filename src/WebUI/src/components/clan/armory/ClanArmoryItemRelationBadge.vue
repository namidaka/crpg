<script setup lang="ts">
import { type UserPublic } from '@/models/user';
import { useUserStore } from '@/stores/user';

const { lender, borrower } = defineProps<{
  lender: UserPublic;
  borrower?: UserPublic | null;
}>();

const { user } = toRefs(useUserStore());
</script>

<template>
  <div class="group flex items-center">
    <VTooltip :class="{ 'transition-transform group-hover:-translate-x-3.5': borrower }">
      <UserMedia
        :user="lender"
        hiddenPlatform
        hiddenTitle
        hiddenClan
        :isSelf="user!.id === lender.id"
      />
      <template #popper>
        <div class="flex items-center gap-2">
          <i18n-t
            scope="global"
            keypath="clan.armory.item.lender.tooltip.title"
            tag="div"
            class="flex items-center gap-2"
          >
            <template #user>
              <UserMedia
                class="max-w-[10rem]"
                :user="lender"
                :isSelf="user!.id === lender.id"
                size="xl"
                hiddenPlatform
                hiddenClan
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
        hiddenClan
        class="relative z-10 -ml-2.5"
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
                size="xl"
                hiddenPlatform
                hiddenClan
              />
            </template>
          </i18n-t>
        </div>
      </template>
    </VTooltip>
  </div>
</template>
