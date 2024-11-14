<script setup lang="ts">
import type { UserPublic } from '~/models/user'

import { useUserStore } from '~/stores/user'

const { borrower, lender } = defineProps<{
  lender: UserPublic
  borrower?: UserPublic | null
}>()

const { user } = toRefs(useUserStore())
</script>

<template>
  <div class="group flex items-center">
    <VTooltip :class="{ 'transition-transform group-hover:-translate-x-3.5': borrower }">
      <UserMedia
        :user="lender"
        hidden-platform
        hidden-title
        hidden-clan
        :is-self="user!.id === lender.id"
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
                class="max-w-40"
                :user="lender"
                :is-self="user!.id === lender.id"
                size="xl"
                hidden-platform
                hidden-clan
              />
            </template>
          </i18n-t>
        </div>
      </template>
    </VTooltip>

    <VTooltip v-if="borrower">
      <UserMedia
        :user="borrower"
        hidden-platform
        hidden-title
        hidden-clan
        class="relative z-10 -ml-2.5"
        :is-self="user!.id === borrower.id"
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
                class="max-w-40"
                :user="borrower"
                :is-self="user!.id === borrower.id"
                size="xl"
                hidden-platform
                hidden-clan
              />
            </template>
          </i18n-t>
        </div>
      </template>
    </VTooltip>
  </div>
</template>
