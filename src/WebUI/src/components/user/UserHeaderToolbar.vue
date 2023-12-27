<script setup lang="ts">
import { useTransition } from '@vueuse/core';
import { useUserStore } from '@/stores/user';
import { mapUserToUserPublic } from '@/services/users-service';
import { logout } from '@/services/auth-service';

const userStore = useUserStore();

const animatedUserGold = useTransition(toRef(() => userStore.user!.gold));
</script>

<template>
  <div class="gap flex items-center gap-3">
    <!-- TODO: improve tooltip, share heirloom, bla bla bla -->
    <Coin :value="Number(animatedUserGold.toFixed(0))" v-tooltip.bottom="$t('user.field.gold')" />

    <Divider inline />

    <Heirloom
      :value="userStore.user!.heirloomPoints"
      v-tooltip.bottom="$t('user.field.heirloom')"
    />

    <Divider inline />

    <UserMedia
      :user="mapUserToUserPublic(userStore.user!, userStore.clan)"
      :clan="userStore.clan"
      :clanRole="userStore.clanMemberRole"
      hiddenPlatform
      size="xl"
    />

    <Divider inline />

    <VDropdown placement="bottom-end">
      <template #default="{ shown }">
        <OButton
          :variant="shown ? 'transparent-active' : 'transparent'"
          size="sm"
          rounded
          iconLeft="dots"
        />
      </template>

      <template #popper="{ hide }">
        <SwitchLanguageDropdown #default="{ shown, locale }" placement="left-start">
          <DropdownItem :active="shown">
            <SvgSpriteImg :name="`locale-${locale}`" viewBox="0 0 18 18" class="w-4.5" />
            {{ $t('setting.language') }} | {{ locale.toUpperCase() }}
          </DropdownItem>
        </SwitchLanguageDropdown>

        <DropdownItem
          tag="RouterLink"
          :to="{ name: 'Settings' }"
          icon="settings"
          :label="$t('setting.settings')"
          @click="hide"
        />

        <DropdownItem
          icon="logout"
          :label="$t('setting.logout')"
          @click="
            () => {
              hide();
              logout();
            }
          "
        />
      </template>
    </VDropdown>
  </div>
</template>
