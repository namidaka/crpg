<script setup lang="ts">
import { clsx } from 'clsx';

type FormGroupVariant = 'primary' | 'danger';

interface FormGroupTheme {
  variant: Record<
    FormGroupVariant,
    {
      wrap: string;
      label: string;
      collapseIcon: string;
    }
  >;
}

const {
  collapsed = false,
  collapsable = true,
  bordered = true,
  variant = 'primary',
} = defineProps<{
  variant?: FormGroupVariant;
  icon?: string;
  label?: string;
  collapsed?: boolean;
  collapsable?: boolean;
  bordered?: boolean;
}>();

const collapsedModel = ref<boolean>(collapsed);

const theme: FormGroupTheme = /*@tw*/ {
  variant: {
    primary: {
      wrap: 'border-border-200',
      label: '',
      collapseIcon: 'text-content-400 group-hover:text-content-100',
    },
    danger: {
      wrap: 'border-status-danger border-2 border-dashed',
      label: 'text-status-danger',
      collapseIcon: 'text-status-danger/80 group-hover:text-status-danger/100',
    },
  },
};

const formGroupClass = /*@tw*/ computed(() =>
  clsx(theme.variant[variant].wrap, bordered && 'border')
);

const formGroupLabelClass = /*@tw*/ computed(() => clsx(theme.variant[variant].label));

const formGroupCollapseIconClass = /*@tw*/ computed(() =>
  clsx(theme.variant[variant].collapseIcon)
);
</script>

<template>
  <div class="rounded-3xl px-6 py-7" :class="formGroupClass">
    <div
      class="group flex items-center justify-between gap-4 text-content-100"
      :class="{ 'cursor-pointer': collapsable }"
      @click="collapsable && (collapsedModel = !collapsedModel)"
    >
      <div class="flex w-full items-center gap-3 text-title-lg" :class="formGroupLabelClass">
        <OIcon v-if="icon" :icon="icon" size="lg" />
        <slot name="label">
          {{ label }}
        </slot>
      </div>

      <OIcon
        v-if="collapsable"
        icon="chevron-down"
        size="lg"
        :class="[{ 'rotate-180': !collapsedModel }, formGroupCollapseIconClass]"
      />
    </div>

    <div v-if="!collapsedModel" class="mt-7 flex flex-col gap-4">
      <slot name="default" />
    </div>
  </div>
</template>
