<script setup lang="ts">
const props = withDefaults(defineProps<{ patreonExpanded?: boolean, size?: string }>(), {
  patreonExpanded: false,
  size: 'xl',
})

interface SocialLink {
  id: string
  href: string
  icon: string
  title: string
}

const socialsLinks: SocialLink[] = [
  {
    href: 'https://www.patreon.com/crpg',
    icon: 'patreon',
    id: 'patreon',
    title: 'Patreon',
  },
  {
    href: 'https://discord.gg/c-rpg',
    icon: 'discord',
    id: 'discord',
    title: 'Discord',
  },
  {
    href: 'https://www.reddit.com/r/CRPG_Bannerlord',
    icon: 'reddit',
    id: 'reddit',
    title: 'Reddit',
  },
  {
    href: 'https://www.moddb.com/mods/crpg',
    icon: 'moddb',
    id: 'moddb',
    title: 'Moddb',
  },
  {
    href: 'https://steamcommunity.com/sharedfiles/filedetails/?id=2878356589',
    icon: 'steam',
    id: 'steam',
    title: 'Steam',
  },
  {
    href: 'https://github.com/namidaka/crpg',
    icon: 'github',
    id: 'github',
    title: 'Github',
  },
]

const links = computed(() =>
  props.patreonExpanded ? socialsLinks.filter(l => l.id !== 'patreon') : socialsLinks,
)

const patreonLink = computed(() => socialsLinks.find(l => l.id === 'patreon')!)
</script>

<template>
  <div class="flex flex-wrap items-center gap-6">
    <template v-if="patreonExpanded">
      <div v-html="$t('patreon')" />

      <OButton
        variant="secondary"
        :size="size"
        outlined
        tag="a"
        :icon-left="patreonLink.icon"
        :href="patreonLink.href"
        target="_blank"
        label="Patreon"
      />

      <div class="h-8 w-px select-none bg-border-200" />
    </template>

    <div class="flex flex-wrap items-center gap-4">
      <OButton
        v-for="social in links"
        :key="social.id"
        v-tooltip.bottom="social.title"
        variant="secondary"
        :size="size"
        outlined
        rounded
        tag="a"
        :icon-left="social.icon"
        :href="social.href"
        target="_blank"
      />
    </div>
  </div>
</template>
