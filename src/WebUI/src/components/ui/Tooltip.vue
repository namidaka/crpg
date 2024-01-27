<script setup lang="ts">
const {
  label,
  title,
  description,
  disabled = false,
} = defineProps<{
  label?: string;
  title?: string | null;
  description?: string | null;
  disabled?: boolean;
}>();

const isDisabled = computed(() => disabled || !title);
</script>

<template>
  <VTooltip placement="auto" :disabled="isDisabled">
    <div>
      <slot>{{ label }}</slot>
    </div>
    <template #popper>
      <div class="prose prose-invert space-y-3">
        <h5>
          {{ title }}
        </h5>
        <div v-if="description" v-html="description" class="text-2xs" />
      </div>
    </template>
  </VTooltip>
</template>
