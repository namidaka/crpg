<script setup lang="ts">
import { useMagicKeys, whenever } from '@vueuse/core';
import type { StepEntity } from 'v-onboarding/src/types/StepEntity';

import { VOnboardingStep } from 'v-onboarding';

interface Step extends StepEntity {
  tags: string[];
}

const emit = defineEmits<{
  next: [];
  previous: [];
  exit: [];
}>();

defineProps<{
  step: Step;
  isFirst: boolean;
  isLast: boolean;
  index: number;
  stepsCount: number;
}>();

const { escape, arrowRight, arrowLeft } = useMagicKeys();

whenever(escape, () => emit('exit'));
whenever(arrowRight, () => emit('next'));
whenever(arrowLeft, () => emit('previous'));
</script>

<template>
  <VOnboardingStep>
    <div class="relative min-w-[24rem] max-w-[30rem] rounded-lg bg-base-300">
      <OButton
        class="!absolute right-4 top-4"
        iconRight="close"
        rounded
        size="2xs"
        variant="secondary"
        @click="$emit('exit')"
      />

      <header class="px-6 pt-4">
        <Tag v-for="tag in step.tags" variant="info" :label="tag" />
      </header>

      <div class="prose prose-invert px-6 py-2">
        <h3 v-if="step.content.title">
          {{ step.content.title }}
        </h3>
        <div v-if="step.content.description" v-html="step.content.description" />
      </div>

      <footer class="px-6 pb-4 pt-2">
        <div class="flex items-center justify-end gap-2">
          <OButton
            v-if="!isFirst"
            variant="secondary"
            size="sm"
            :label="`Previous`"
            iconLeft="arrow-left"
            @click="$emit('previous')"
          />

          <OButton
            variant="primary"
            size="sm"
            :label="isLast ? 'Finish' : 'Next'"
            :iconRight="!isLast ? 'arrow-right' : undefined"
            @click="$emit('next')"
          />
        </div>
      </footer>

      <div class="border-t border-border-300 px-4 py-2">
        <div class="flex items-center justify-between gap-8">
          <div class="text-primary">
            {{ `${index + 1}/${stepsCount}` }}
          </div>

          <div class="flex items-center gap-4">
            <KbdGroup :keys="['→', '←']" :label="`to navigate`" />
            <KbdGroup :keys="['ESC']" :label="`to close`" />
          </div>
        </div>
      </div>
    </div>
  </VOnboardingStep>
</template>
