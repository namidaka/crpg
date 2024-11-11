<script setup lang="ts">
import { getRestrictions } from '~/services/restriction-service'

definePage({
  meta: {
    roles: ['Moderator', 'Admin'],
  },
})

const { execute: loadRestrictions, state: restrictions } = useAsyncState(
  () => getRestrictions(),
  [],
  {
    immediate: false,
  },
)

await loadRestrictions()
</script>

<template>
  <div class="container">
    <div class="mb-16">
      <RestrictionsTable :restrictions="restrictions" />
    </div>
  </div>
</template>
