<script setup lang="ts">
import { currentLocale, supportedLocales, switchLanguage } from '~/services/translate-service'

const locale = computed(() => currentLocale())
const locales = supportedLocales()
</script>

<template>
  <VDropdown
    :triggers="['click']"
    placement="bottom-end"
  >
    <template #default="scope">
      <slot v-bind="{ ...scope, locale }" />
    </template>

    <template #popper="{ hide }">
      <DropdownItem
        v-for="l in locales"
        :key="l"
        :checked="l === locale"
        data-aq-switch-lang-item
        @click="
          () => {
            switchLanguage(l);
            hide();
          }
        "
      >
        <SvgSpriteImg
          :name="`locale-${l}`"
          viewBox="0 0 18 18"
          class="w-4.5"
        />
        {{ $t(`locale.${l}`) }}
      </DropdownItem>
    </template>
  </VDropdown>
</template>
