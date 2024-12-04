<script setup lang="ts">
import type { Settings } from '~/models/setting'

import { editSettings } from '~/services/settings-service'
import { useSettingsStore } from '~/stores/settings'
import { useAsyncCallback } from '~/utils/useAsyncCallback'

definePage({
  meta: {
    roles: ['Admin'],
  },
})

const settingStore = useSettingsStore()

const { execute: onEditSettings, loading: editingSetting } = useAsyncCallback(async (settings: Partial<Settings>) => {
  await editSettings(settings)
  await settingStore.loadSettings()
})
</script>

<template>
  <div class="container">
    <div class="mx-auto py-12">
      <h1 class="mb-14 text-center text-xl text-content-100">
        {{ $t('nav.main.Admin') }}
      </h1>

      <OLoading
        v-if="settingStore.isLoadingSettings"
        active
        icon-size="xl"
      />

      <SettingsForm
        :settings="settingStore.settings"
        :loading="editingSetting"
        @reset="settingStore.loadSettings"
        @submit="onEditSettings"
      />
    </div>
  </div>
</template>
