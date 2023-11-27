<script setup lang="ts">
import { defaultGold } from '@root/data/constants.json';

import { Platform } from '@/models/platform';
import { usePlatform } from '@/composables/use-platform';

defineEmits<{
  start: [];
}>();

// TODO: FIXME:
const { platform } = usePlatform();

enum PossibleValues {
  Steam = 'Steam',
  Other = 'Other',
}

const tabsModel = ref<PossibleValues>(PossibleValues.Steam);

watch(
  () => platform.value,
  () => {
    tabsModel.value =
      platform.value === Platform.Steam ? PossibleValues.Steam : PossibleValues.Other;
  },
  {
    immediate: true,
  }
);
</script>

<template>
  <Modal closable shown>
    <template #popper>
      <div class="w-[40rem] space-y-10">
        <div class="relative h-[10rem]">
          <!-- TODO: poster -->
          <img
            class="absolute inset-0 aspect-video h-full w-full object-cover opacity-50"
            :src="`/images/bg/background-1.webp`"
          />

          <!-- TODO: heading cmp from clan-armory branch -->
          <div
            class="tem-center absolute left-1/2 top-1/2 flex w-full -translate-x-1/2 -translate-y-1/2 select-none justify-center gap-8 text-center"
          >
            <SvgSpriteImg
              name="logo-decor"
              viewBox="0 0 108 10"
              class="w-24 rotate-180 transform"
            />
            <h2 class="text-2xl text-white">Welcome warrior</h2>
            <SvgSpriteImg name="logo-decor" viewBox="0 0 108 10" class="w-24" />
          </div>
        </div>

        <div class="prose prose-invert space-y-10 px-12">
          <OTabs v-model="tabsModel" size="xl" :animated="false">
            <OTabItem :label="`Intro`" :value="PossibleValues.Steam">
              <!-- TODO: -->
              <div class="grid auto-cols-[8rem_auto] grid-flow-col">
                <div>ddd</div>
                <div>
                  <p>TODO: Builder</p>
                  <p>TODO: Rules</p>
                  <p>TODO: tips-tricks-n-help</p>

                  <p>
                    cRPG gives you the freedom to create whatever character you want. On your way to
                    level 35 you can customize your stats and buy your own equipment. But this also
                    means that you can end up with a terrible character for multiplayer gameplay!
                  </p>

                  <!-- TODO: -->
                  Вы получили
                  <Coin :value="defaultGold" />

                  <!-- <div class="space-y-6">
                <ol>
                  <li>TODO:</li>
                  <li>TODO:</li>
                  <li>TODO:</li>
                  <li>TODO:</li>
                </ol>
                <p class="text-content-400">TODO:</p>
              </div> -->
                </div>
              </div>
            </OTabItem>

            <OTabItem :label="`TODO:`" :value="PossibleValues.Other">
              <div>
                <ol>
                  <li>TODO:</li>
                  <li>TODO:</li>
                  <li>TODO:</li>
                  <li>TODO:</li>
                </ol>
                <p class="text-content-400">TODO:</p>
              </div>
            </OTabItem>
          </OTabs>
        </div>

        <OButton
          variant="primary"
          outlined
          size="xl"
          :label="`TODO: Start`"
          @click="$emit('start')"
        />

        <Divider />

        <div class="prose prose-invert px-12">
          <p class="text-content-400">TODO:</p>
        </div>
      </div>
    </template>
  </Modal>
</template>
