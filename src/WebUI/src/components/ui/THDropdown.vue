<script setup lang="ts">
defineProps<{
  shownReset: boolean;
  label?: string;
}>();

defineEmits<{
  reset: [];
}>();
</script>

<template>
  <div class="relative flex items-center gap-1">
    <OIcon
      v-if="shownReset"
      class="absolute -left-5 top-1/2 -translate-y-1/2 transform cursor-pointer hover:text-status-danger"
      v-tooltip.bottom="$t('action.reset')"
      icon="close"
      size="xs"
      @click="$emit('reset')"
    />

    <VDropdown :triggers="['click']">
      <div
        class="underline-offset-6 max-w-[90px] cursor-pointer select-none overflow-x-hidden text-ellipsis whitespace-nowrap text-2xs leading-loose underline decoration-dashed hover:text-content-100 hover:no-underline 2xl:max-w-[120px]"
      >
        <slot name="label">{{ label }}</slot>
      </div>

      <template #popper="{ hide }">
        <div class="max-h-64 max-w-md overflow-y-auto">
          <slot v-bind="{ hide }" />
        </div>
      </template>
    </VDropdown>

    <slot name="labelAppend" />
  </div>
</template>
