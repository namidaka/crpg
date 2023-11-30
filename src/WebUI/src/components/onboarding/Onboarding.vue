<script setup lang="ts">
import { VOnboardingWrapper, useVOnboarding } from 'v-onboarding';
import type { VOnboardingWrapperOptions } from 'v-onboarding/src/types/VOnboardingWrapper';
import { asyncPoll } from '@/utils/poll';

const router = useRouter();

const wrapper = ref(null);
const { start, goToStep, finish } = useVOnboarding(wrapper);

const tryFindAttachToElement = (elSelector: string) =>
  asyncPoll(
    async () =>
      Promise.resolve({
        done: await Promise.resolve(Boolean(document.querySelector(elSelector))),
      }),
    50,
    10
  );

// TODO:

/*
Default Layout
  - header
    - online players
    - gold
    - heirlooms
    - settings
  - footer
    - socials, patreon
    - HH timetable

Character
  - selector: select, active, create new
  - settings (edit, delete)
  - sub-pages
    - overview
      - common area (Overview)
      - actions
       - respec
       - retire
       - tournament
    - inventory (empty?)
      - items grid
        - sort/search/filters
        - drag&drop
        - click - detail window
      - doll
      - stats
    - characteristic
      - common tips/links
      - convert
    - builder?
      - TODO
    - stats (exp/gold)
      - TODO

Clan
  - create, join, armory, team balance

*/

const steps = [
  {
    attachTo: { element: '[data-o8="common-layout-header-online-players"]' },
    tags: ['common'],
    content: {
      title: 'TODO:',
      description: 'TODO:',
    },
  },
  {
    attachTo: { element: '[data-o8="common-layout-header-user-gold"]' },
    tags: ['common'],
    content: {
      title: 'TODO:',
      description: 'TODO:',
    },
  },
  {
    attachTo: { element: '[data-o8="common-layout-header-user-heirloom"]' },
    tags: ['common'],
    content: {
      title: 'TODO:',
      description: 'TODO:',
    },
  },
  {
    attachTo: { element: '[data-o8="common-layout-header-user-media"]' },
    tags: ['common'],
    content: {
      title: 'TODO:',
      description: 'TODO:',
    },
  },
  {
    attachTo: { element: '[data-o8="common-layout-header-settings"]' },
    tags: ['common'],
    content: {
      title: 'TODO:',
      description: 'TODO:',
    },
  },
  {
    attachTo: { element: '[data-o8="common-layout-footer-socials"]' },
    tags: ['common'],
    content: {
      title: 'TODO:',
      description: 'TODO:',
    },
  },
  {
    attachTo: { element: '[data-o8="common-layout-footer-hh-timetable"]' },
    tags: ['common'],
    content: {
      title: 'TODO:',
      description: 'TODO:',
    },
  },
  // {
  //   attachTo: { element: '[data-s-d22]' },
  //   tags: ['common'],
  //   content: {
  //     title: 'TODO:',
  //     description: 'TODO:',
  //   },
  //   on: {
  //     beforeStep: async (options: any) => {
  //       await router.push({ name: 'Clans' });
  //       await tryFindAttachToElement(options.step.attachTo.element);
  //     },
  //   },
  // },
];

const wrapperOptions = {
  popper: {
    modifiers: [
      {
        name: 'offset',
        options: {
          offset: [0, 10],
        },
      },
    ],
  },
} as VOnboardingWrapperOptions;

onMounted(start);
</script>

<template>
  <div class="fixed bottom-24 right-0 z-10">
    <OButton variant="primary" outlined size="xl" iconLeft="tag" :label="`Start onboarding`" />
  </div>

  <VOnboardingWrapper ref="wrapper" :steps="steps" :options="wrapperOptions">
    <template #default="{ previous, next, step, exit, isFirst, isLast, index }">
      <OnboardingStep
        v-if="step"
        v-bind="{ step, isFirst, isLast, index, stepsCount: steps.length }"
        @next="next"
        @previous="previous"
        @exit="finish"
      />
    </template>
  </VOnboardingWrapper>
</template>
