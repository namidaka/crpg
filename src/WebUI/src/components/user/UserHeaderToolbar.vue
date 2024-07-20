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
        <OButton :variant="shown ? 'transparent-active' : 'transparent'" size="sm" rounded>
          <FontAwesomeLayers full-width class="fa-2x">
            <FontAwesomeIcon :icon="['crpg', 'carillon']" />
            <!-- v-if="active" - has unread -->
            <FontAwesomeLayersText
              counter
              value="●"
              position="top-right"
              :style="{ '--fa-counter-background-color': 'rgba(83, 188, 150, 1)' }"
            />
          </FontAwesomeLayers>
        </OButton>
      </template>

      <!-- TODO: BaseCard/Island component -->
      <template #popper="{ hide }">
        <div class="w-[26rem]">
          <div class="prose prose-invert px-5 py-3">
            <h5 class="mb-0">Notifications</h5>
          </div>

          <Divider class="stroke-current text-base-500" />

          <NotificationCard
            v-if="Boolean(userStore.notifications.length)"
            v-for="notification in userStore.notifications"
            :notification="notification"
          />
          <div v-else class="px-5 py-3">У вас еще нет уведомлений.</div>
        </div>
      </template>
    </VDropdown>

    <VDropdown placement="bottom-end">
      <template #default="{ shown }">
        <OButton :variant="shown ? 'transparent-active' : 'transparent'" size="sm" rounded>
          <FontAwesomeLayers full-width class="fa-2x">
            <FontAwesomeIcon :icon="['crpg', 'dots']" />
          </FontAwesomeLayers>
        </OButton>
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
