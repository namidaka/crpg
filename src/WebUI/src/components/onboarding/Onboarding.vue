<script setup lang="ts">
import { VOnboardingWrapper, useVOnboarding } from 'v-onboarding';
import { asyncPoll } from '@/utils/poll';

const router = useRouter();

const wrapper = ref(null);
const { start, goToStep, finish } = useVOnboarding(wrapper);

const tryFindAttachToElement = async (selector: string) =>
  Promise.resolve(Boolean(document.querySelector(selector)));

const steps = shallowRef([
  {
    attachTo: { element: '[data-s-d1]' },
    content: {
      title: 'Welcome!',
      description:
        'Lorem ipsum dolor sit amet consectetur adipisicing elit. Reprehenderit atque aliquam, sint dolorem amet soluta ut ipsa dolorum harum placeat.',
    },
  },
  {
    attachTo: { element: '[data-s-d22]' },
    content: {
      title: 'Welcome!22222222',
      description:
        'Lorem ipsum dolor sit amet consectetur adipisicing elit. Reprehenderit atque aliquam, sint dolorem amet soluta ut ipsa dolorum harum placeat.',
    },
    on: {
      beforeStep: async (options: any) => {
        await router.push({ name: 'Clans' });
        await asyncPoll(
          async () => {
            return Promise.resolve({
              done: await tryFindAttachToElement(options.step.attachTo.element),
            });
          },
          50,
          10
        );
      },
    },
  },
  {
    attachTo: { element: '[data-s-d33]' },
    content: { title: 'Welcome!333333333' },
    on: {
      beforeStep: async (options: any) => {
        await router.push({ name: 'Shop' });
        await asyncPoll(
          async () => {
            return Promise.resolve({
              done: await tryFindAttachToElement(options.step.attachTo.element),
            });
          },
          50,
          10
        );
      },
    },
  },
]);

onMounted(async () => {
  await nextTick();
  start();
});
</script>

<template>
  <VOnboardingWrapper ref="wrapper" :steps="steps">
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
