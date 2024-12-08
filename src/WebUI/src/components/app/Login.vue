<script setup lang="ts">
import { usePlatform } from '~/composables/use-platform'
import { useAsyncCallback } from '~/composables/utils/use-async-callback'
import { Platform } from '~/models/platform'
import { login } from '~/services/auth-service'
import { platformToIcon } from '~/services/platform-service'
import { useUserStore } from '~/stores/user'

const { user } = toRefs(useUserStore())
const { changePlatform, platform } = usePlatform()
const { execute: loginUser, loading: logging } = useAsyncCallback(() => login(platform.value))
</script>

<template>
  <OField v-if="user === null">
    <OButton
      variant="primary"
      size="xl"
      :icon-left="platformToIcon[platform]"
      :label="$t(`platform.${platform}`)"
      :loading="logging"
      data-aq-login-btn
      @click="loginUser"
    >
      <div class="flex flex-col text-left leading-tight">
        <span class="text-[10px]">{{ $t('login.label') }}</span>
        <span>{{ $t(`platform.${platform}`) }}</span>
      </div>
    </OButton>

    <VDropdown
      :triggers="['click']"
      placement="bottom-end"
    >
      <template #default="{ shown }">
        <OButton
          variant="primary"
          :icon-right="shown ? 'chevron-up' : 'chevron-down'"
          size="xl"
        />
      </template>

      <template #popper="{ hide }">
        <DropdownItem
          v-for="p in Object.values(Platform)"
          :key="p"
          :checked="p === platform"
          :label="$t(`platform.${p}`)"
          :icon="platformToIcon[p]"
          data-aq-platform-item
          @click="
            () => {
              changePlatform(p);
              hide();
            }
          "
        />
      </template>
    </VDropdown>
  </OField>

  <RouterLink
    v-else
    :to="{ name: 'Characters' }"
    data-aq-character-link
  >
    <OButton
      variant="primary"
      size="xl"
      icon-left="member"
      :label="$t('action.enter')"
    />
  </RouterLink>
</template>
