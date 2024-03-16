<script lang="ts" setup>
definePage({
  props: true,
  meta: {
    roles: ['User', 'Moderator', 'Admin'],
  },
});
</script>

<template>
  <div class="mx-auto max-w-2xl space-y-12 pb-12">
    <CharacterEarningChart />
  </div>
</template>
