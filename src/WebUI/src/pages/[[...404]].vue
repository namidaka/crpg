<script setup lang="ts">
definePage({
  meta: {
    layout: 'empty',
    skipAuth: true,
  },
  name: 'PageNotFound',
})
</script>

<template>
  <div>
    <!-- TODO: Custom design -->
    {{ $t('not-found') }}
  </div>
</template>
