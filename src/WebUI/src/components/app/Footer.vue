<script setup lang="ts">
import { type HHEvent } from '@/services/hh-service';
import { useUserStore } from '@/stores/user';
import { scrollToTop } from '@/utils/scroll';

defineProps<{ HHEvent: HHEvent }>();

const userStore = useUserStore();
</script>

<template>
  <footer
    class="relative mt-auto flex flex-wrap items-center justify-between border-t border-solid border-border-200 px-6 py-5 text-2xs text-content-300"
  >
    <Socials patreonExpanded size="sm" />

    <div class="flex items-center gap-5">
      <HHTooltip #default="{ shown }" :region="userStore.user!.region">
        <div
          class="group flex cursor-pointer select-none items-center gap-2 hover:text-content-100"
          :class="{ 'text-content-100': shown }"
        >
          <OIcon icon="gift" size="lg" class="text-content-100" />
          {{
            $t('hh.tooltip-trigger', {
              region: $t(`region.${userStore.user!.region}`, 1),
            })
          }}
          <span
            class="group-hover:text-content-100"
            :class="[shown ? 'text-content-100' : 'text-content-200']"
          >
            {{ $d(HHEvent.start, 'time') }} - {{ $d(HHEvent.end, 'time') }}
          </span>
        </div>
      </HHTooltip>

      <Divider inline />

      <OButton
        v-tooltip="$t('scrollToTop')"
        variant="transparent"
        size="xl"
        iconRight="arrow-up"
        rounded
        @click="scrollToTop"
      />
    </div>
  </footer>
</template>
