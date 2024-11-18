<script setup lang="ts">
definePage({
  meta: {
    layout: 'default',
    roles: ['Moderator', 'Admin'],
  },
})
</script>

<template>
  <div class="py-6">
    <div class="mb-12 flex items-center justify-center gap-2">
      <RouterLink
        v-slot="{ isExactActive }"
        :to="{ name: 'Moderator' }"
      >
        <OButton
          :variant="isExactActive ? 'transparent-active' : 'transparent'"
          size="lg"
          :label="$t('restriction.title')"
        />
      </RouterLink>

      <RouterLink
        v-slot="{ isExactActive }"
        :to="{ name: 'ModeratorFindUser' }"
      >
        <OButton
          :variant="isExactActive ? 'transparent-active' : 'transparent'"
          size="lg"
          :label="$t('findUser.title')"
        />
      </RouterLink>
    </div>

    <RouterView />
  </div>
</template>
