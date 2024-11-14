<script setup lang="ts">
import { clsx } from 'clsx'

type BadgeVariant = 'primary' | 'info' | 'success' | 'warning' | 'danger'
type BadgeSize = 'sm' | 'lg'

interface BadgeTheme {
  base: string
  size: Record<BadgeSize, string>
  square: Record<BadgeSize, string>
  padding: Record<BadgeSize, string>
  iconSize: Record<BadgeSize, string>
  variant: Record<BadgeVariant, string>
}

const {
  disabled = false,
  padded = true,
  rounded = false,
  size = 'sm',
  variant = 'info',
} = defineProps<{
  variant?: BadgeVariant
  size?: BadgeSize
  rounded?: boolean
  disabled?: boolean
  padded?: boolean
  label?: string
  icon?: string
}>()

const theme: BadgeTheme = /* @tw */ {
  base: '!inline-flex cursor-pointer items-center justify-center',
  iconSize: {
    lg: 'sm',
    sm: 'xs',
  },
  padding: {
    lg: 'px-1.5 py-0.5',
    sm: 'px-1.5 py-0.5',
  },
  size: {
    lg: 'text-2xs',
    sm: 'text-3xs',
  },
  square: {
    lg: 'h-7 w-7',
    sm: 'h-5 w-5',
  },
  variant: {
    danger: 'bg-base-200 text-status-danger hover:bg-status-danger hover:text-content-600',
    info: 'bg-base-200 text-content-200 hover:bg-base-500 hover:text-content-100',
    primary: 'bg-base-200 text-primary hover:bg-base-300 hover:text-primary-hover',
    success: 'bg-base-200 text-status-success hover:bg-status-success hover:text-content-600',
    warning: 'bg-base-200 text-status-warning hover:bg-status-warning hover:text-content-600',
  },
}

const badgeClass = /* @tw */ computed(() => {
  return clsx(
    theme.base,
    theme.size[size],
    padded && theme[rounded ? 'square' : 'padding'][size],
    rounded ? 'rounded-full' : 'rounded-md',
    theme.variant[variant],
    disabled && 'pointer-events-none',
  )
})

const iconSize = computed(() => theme.iconSize[size])
</script>

<template>
  <div
    class="!inline-flex cursor-pointer items-center justify-center gap-1 rounded-full"
    :class="badgeClass"
  >
    <OIcon
      v-if="icon"
      :icon="icon"
      :size="iconSize"
    />
    <template v-if="label">
      {{ label }}
    </template>
  </div>
</template>
