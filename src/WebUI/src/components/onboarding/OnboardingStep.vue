<script setup lang="ts">
import { VOnboardingStep } from 'v-onboarding';

interface Step {
  content: {
    title: string;
    description?: string;
  };
}

defineEmits<{
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
</script>

<template>
  <VOnboardingStep>
    <div class="relative max-w-[30rem] rounded-lg bg-base-300 p-6">
      <OButton
        class="!absolute right-4 top-4"
        iconRight="close"
        rounded
        size="2xs"
        variant="secondary"
        @click="$emit('exit')"
      />

      <div class="space-y-4">
        <div class="prose prose-invert">
          <h3 v-if="step.content.title">
            {{ step.content.title }}
          </h3>

          <div v-if="step.content.description">
            <p>{{ step.content.description }}</p>
          </div>
        </div>

        <div class="relative">
          <div class="flex items-center justify-between gap-2">
            <div class="text-primary">
              {{ `${index + 1}/${stepsCount}` }}
            </div>

            <div class="flex items-center gap-2">
              <OButton
                v-if="!isFirst"
                variant="secondary"
                size="xl"
                :label="`Previous`"
                iconLeft="arrow-left"
                @click="$emit('previous')"
              />

              <OButton
                variant="primary"
                size="xl"
                :label="isLast ? 'Finish' : 'Next'"
                :iconRight="!isLast ? 'arrow-right' : undefined"
                @click="$emit('next')"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </VOnboardingStep>
</template>
