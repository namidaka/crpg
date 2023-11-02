<script setup lang="ts">
import { type UserPublic } from '@/models/user';

const { owner } = defineProps<{ owner?: UserPublic | null }>();
</script>

<template>
  <VTooltip v-if="owner">
    <UserMedia :user="owner" hiddenPlatform hiddenTitle />
    <template #popper>
      <div class="flex items-center gap-2">
        <i18n-t
          scope="global"
          keypath="clan.armory.item.owner.tooltip.title"
          tag="div"
          class="flex items-center gap-2"
        >
          <template #user>
            <UserMedia class="max-w-[10rem]" :user="owner" hiddenPlatform />
          </template>
        </i18n-t>
      </div>
    </template>
  </VTooltip>

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
