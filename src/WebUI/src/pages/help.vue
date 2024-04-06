<script setup lang="ts">
import type { RouteNamedMap } from 'vue-router/auto-routes';
import { t } from '@/services/translate-service';

definePage({
  meta: {
    layout: 'default',
    bg: 'background-3.webp',
  },
});

const helpHubNavigation: { name: keyof RouteNamedMap; label: string }[] = [
  {
    name: 'Help',
    label: t('help.overview.navTitle'),
  },
  {
    name: 'HelpBuildSupport',
    label: t('help.buildSupport.navTitle'),
  },
  {
    name: 'HelpJoinClan',
    label: t('help.joinClan.navTitle'),
  },
  {
    name: 'HelpFaq',
    label: t('help.FAQ.navTitle'),
  },
];
</script>

<template>
  <div class="container py-8 md:py-16">
    <Heading :title="$t('help.title')" />

    <div class="mb-12 flex items-center justify-center gap-2">
      <!-- TODO: ts -->
      <RouterLink
        v-for="{ name, label } in helpHubNavigation"
        :to="{ name }"
        v-slot="{ isExactActive }"
      >
        <OButton :variant="isExactActive ? 'transparent-active' : 'secondary'" size="lg" :label />
      </RouterLink>
    </div>

    <div class="mx-auto max-w-4xl">
      <RouterView />
    </div>
  </div>
</template>
