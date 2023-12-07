<script setup lang="ts">
import { defaultGold, startedCharacterLevel } from '@root/data/constants.json';

const emit = defineEmits<{
  close: [];
}>();
</script>

<template>
  <Modal closable :autoHide="false" shown @hide="$emit('close')">
    <template #popper="{ hide }">
      <div class="flex max-h-[90vh] w-[44rem] flex-col">
        <header class="relative min-h-[10rem]">
          <!-- TODO: poster -->
          <img
            class="absolute inset-0 aspect-video h-full w-full object-cover opacity-50"
            :src="`/images/bg/background-3.webp`"
          />
          <!-- TODO: heading cmp from clan-armory branch -->
          <div class="absolute left-1/2 top-1/2 w-full -translate-x-1/2 -translate-y-1/2 space-y-2">
            <div class="flex justify-center">
              <SvgSpriteImg name="logo" viewBox="0 0 162 124" class="w-16" />
            </div>
            <div class="flex select-none items-center justify-center gap-8 text-center">
              <SvgSpriteImg
                name="logo-decor"
                viewBox="0 0 108 10"
                class="w-24 rotate-180 transform"
              />

              <h2 class="text-2xl text-white">{{ $t('welcome.title') }}</h2>
              <SvgSpriteImg name="logo-decor" viewBox="0 0 108 10" class="w-24" />
            </div>
          </div>
        </header>

        <div class="h-full space-y-10 overflow-y-auto px-12 py-8">
          <div
            class="prose prose-invert text-center prose-p:my-1 prose-p:text-2xs"
            v-html="$t('welcome.intro')"
          />

          <div class="relative rounded-3xl border border-border-200 px-6 py-10">
            <div class="absolute left-1/2 top-0 -translate-x-1/2 -translate-y-1/2 bg-base-100 px-3">
              <h4 class="text-sm text-primary">{{ $t('welcome.bonusTitle') }}</h4>
            </div>

            <div class="flex flex-wrap items-center justify-center gap-4 px-20">
              <Coin
                :value="defaultGold"
                v-tooltip="{
                  popperClass: 'prose prose-invert',
                  content: $t('welcome.bonus.gold'),
                  html: true,
                }"
              />
              <Tag
                icon="member"
                variant="primary"
                size="lg"
                :label="`${startedCharacterLevel} lvl`"
                v-tooltip="{
                  popperClass: 'prose prose-invert',
                  content: $t('welcome.bonus.startedCharacter', { level: startedCharacterLevel }),
                  html: true,
                }"
              />
              <Tag
                icon="chevron-down-double"
                variant="primary"
                size="lg"
                label="free respec *"
                v-tooltip="{
                  popperClass: 'prose prose-invert',
                  content: $t('welcome.bonus.freeRespec', { level: startedCharacterLevel + 1 }),
                  html: true,
                }"
              />
            </div>

            <div
              class="absolute bottom-0 left-1/2 flex -translate-x-1/2 translate-y-1/2 justify-center bg-base-100 px-3"
            >
              <OButton variant="primary" size="lg" :label="$t('action.start')" @click="hide" />
            </div>
          </div>

          <div class="space-y-6">
            <FormGroup
              icon="help-circle"
              :label="$t('welcome.helpfulLinks.label')"
              :collapsable="false"
              :collapsed="false"
            >
              <div class="prose prose-invert">
                <ul class="columns-2 items-start">
                  <li>Onboarding (soon)</li>
                  <li>
                    <a
                      target="_blank"
                      href="https://discord.com/channels/279063743839862805/1139507517462937600"
                    >
                      New players readme (en)
                    </a>
                  </li>
                  <li>
                    <a
                      target="_blank"
                      href="https://discord.com/channels/279063743839862805/1034894834378494002"
                    >
                      FAQ (en)
                    </a>
                  </li>
                  <li>
                    <a
                      target="_blank"
                      href="https://discord.com/channels/279063743839862805/1140992563701108796"
                    >
                      Infantry Beginners Guide (en)
                    </a>
                  </li>
                  <li>
                    <a
                      target="_blank"
                      href="https://discord.com/channels/279063743839862805/1036085650849550376"
                    >
                      Character builds
                    </a>
                  </li>
                  <li>
                    <a
                      target="_blank"
                      href="https://discord.com/channels/279063743839862805/761283333840699392"
                    >
                      Tech support
                    </a>
                  </li>
                </ul>
              </div>
            </FormGroup>
          </div>
        </div>

        <footer class="-mt-3">
          <Divider />

          <div class="px-12 py-6">
            <div class="prose prose-invert">
              <p class="text-2xs text-content-400">
                {{ $t('welcome.bonusHint', { level: startedCharacterLevel + 1 }) }}
              </p>
            </div>
          </div>
        </footer>
      </div>
    </template>
  </Modal>
</template>
