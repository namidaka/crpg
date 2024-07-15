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
            <!-- v-if="active" -->
            <FontAwesomeLayersText
              counter
              value="●"
              position="top-right"
              :style="{ '--fa-counter-background-color': 'rgba(83, 188, 150, 1)' }"
            />
          </FontAwesomeLayers>
        </OButton>
      </template>

      <template #popper="{ hide }">
        <!-- TODO: merge with activity log card -->
        <DropdownItem>
          <div class="flex max-w-xl flex-col space-y-2">
            <div class="flex items-center gap-2">
              <!-- TODO: create System User cmp -->
              <div class="flex items-center gap-1.5 text-content-100">
                <SvgSpriteImg name="logo" viewBox="0 0 162 124" class="w-6" />
                System
              </div>

              <div class="text-2xs text-content-300">
                {{ $d(new Date('2024-07-15T20:26:06.2775662Z'), 'short') }}
              </div>
              <Tag variant="primary" :label="'CharacterEarned'" />
            </div>
            <div>
              Lorem ipsum dolor sit amet consectetur, adipisicing elit. Consequuntur tempore at,
              ullam unde nobis vitae atque mollitia dolore laboriosam cum ipsam incidunt saepe odit
              aperiam obcaecati, maxime eum. Aliquid inventore cum culpa!
            </div>
          </div>
        </DropdownItem>

        <Divider />

        <DropdownItem>
          <div class="flex max-w-xl flex-col space-y-2">
            <div class="flex items-center gap-2">
              <div class="flex items-center gap-1.5 text-content-100">
                <SvgSpriteImg name="logo" viewBox="0 0 162 124" class="w-6" />
                System
              </div>
              <div class="text-2xs text-content-300">
                {{ $d(new Date('2024-07-15T20:26:06.2775662Z'), 'long') }}
              </div>
              <Tag variant="primary" :label="'CharacterEarned'" />
            </div>
            <div>
              Lorem ipsum dolor sit amet consectetur, adipisicing elit. Consequuntur tempore at,
              ullam unde nobis vitae atque mollitia dolore laboriosam cum ipsam incidunt saepe odit
              aperiam obcaecati, maxime eum. Aliquid inventore cum culpa!
            </div>
          </div>
        </DropdownItem>
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
