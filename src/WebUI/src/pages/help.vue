<script setup lang="ts">
import type { RouteNamedMap } from 'vue-router/auto-routes'

import { t } from '~/services/translate-service'

definePage({
  meta: {
    bg: 'background-3.webp',
    layout: 'default',
  },
})

const helpHubNavigation: { name: keyof RouteNamedMap, label: string }[] = [
  {
    label: t('help.overview.navTitle'),
    name: 'Help',
  },
  {
    label: t('help.buildSupport.navTitle'),
    name: 'HelpBuildSupport',
  },
  {
    label: t('help.joinClan.navTitle'),
    name: 'HelpJoinClan',
  },
  {
    label: t('help.FAQ.navTitle'),
    name: 'HelpFaq',
  },
]
</script>

<template>
  <div class="container py-8 md:py-16">
    <Heading :title="$t('help.title')" />

    <div class="mb-12 flex items-center justify-center gap-2">
      <RouterLink
        v-for="{ name, label } in helpHubNavigation"
        :key="name"
        v-slot="{ isExactActive }"
        :to="{ name }"
      >
        <OButton
          :variant="isExactActive ? 'transparent-active' : 'secondary'"
          size="lg"
          :label
        />
      </RouterLink>
    </div>

    <div class="mx-auto max-w-4xl">
      <RouterView />
    </div>
  </div>
</template>
