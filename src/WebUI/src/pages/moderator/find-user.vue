<script setup lang="ts">
definePage({
  meta: {
    roles: ['Moderator', 'Admin'],
  },
});
</script>

<template>
  <div class="container">
    <div class="mx-auto max-w-3xl">
      <UserFinder />
    </div>
  </div>
</template>
