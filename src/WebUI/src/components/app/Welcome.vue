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
      <div class="space-y-10 py-10">
        <div class="prose prose-invert space-y-10 px-12">
          <h2 class="text-center text-xl">Welcome!</h2>

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
