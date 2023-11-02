<script setup lang="ts">
import { clsx } from 'clsx';

type BadgeVariant = 'primary' | 'info' | 'success' | 'warning' | 'danger';
type BadgeSize = 'sm' | 'lg';

interface BadgeTheme {
  base: string;
  variant: Record<BadgeVariant, string>;
  size: Record<BadgeSize, string>;
  padding: Record<BadgeSize, string>;
  square: Record<BadgeSize, string>;
  iconSize: Record<BadgeSize, string>;
}

const {
  variant = 'info',
  size = 'sm',
  rounded = false,
  disabled = false,
  padded = true,
} = defineProps<{
  variant?: BadgeVariant;
  size?: BadgeSize;
  rounded?: boolean;
  disabled?: boolean;
  padded?: boolean;
  label?: string;
  icon?: string;
}>();

const theme: BadgeTheme = /*@tw*/ {
  base: '!inline-flex cursor-pointer items-center justify-center',
  size: {
    sm: 'text-3xs',
    lg: 'text-2xs',
  },
  iconSize: {
    sm: 'xs',
    lg: 'sm',
  },
  padding: {
    sm: 'px-1.5 py-0.5',
    lg: 'px-1.5 py-0.5',
  },
  square: {
    sm: 'h-5 w-5',
    lg: 'h-7 w-7',
  },
  variant: {
    primary: 'bg-base-200 text-primary hover:bg-base-300 hover:text-primary-hover',
    info: 'bg-base-200 text-content-200 hover:bg-base-500 hover:text-content-100',
    success: 'bg-base-200 text-status-success hover:bg-status-success hover:text-content-600',
    warning: 'bg-base-200 text-status-warning hover:bg-status-warning hover:text-content-600',
    danger: 'bg-base-200 text-status-danger hover:bg-status-danger hover:text-content-600',
  },
};

const badgeClass = /*@tw*/ computed(() => {
  return clsx(
    theme.base,
    theme.size[size],
    padded && theme[rounded ? 'square' : 'padding'][size],
    rounded ? 'rounded-full' : 'rounded-md',
    theme.variant[variant],
    disabled && 'pointer-events-none'
  );
});

const iconSize = computed(() => theme.iconSize[size]);
</script>

<template>
  <div
    class="!inline-flex cursor-pointer items-center justify-center rounded-full"
    :class="badgeClass"
  >
    <template v-if="label">{{ label }}</template>
    <OIcon v-if="icon" :icon="icon" :size="iconSize" />
  </div>
</template>
