<script setup lang="ts">
import { capitalize } from 'es-toolkit'

import type { Settings } from '~/models/setting'

import { editSettings, getSettings } from '~/services/settings-service'
import { useAsyncCallback } from '~/utils/useAsyncCallback'

definePage({
  meta: {
    roles: ['Admin'],
  },
})

const { execute: loadSettings, state: settings, isLoading: isLoadingSettings } = useAsyncState(
  () => getSettings(),
  {
    discord: '',
    steam: '',
    patreon: '',
    github: '',
    reddit: '',
    modDb: '',
  },
  {
    immediate: true,
  },
)

const { execute: onEditSettings, loading: editingSetting } = useAsyncCallback(async (settings: Partial<Settings>) => {
  await editSettings(settings)
  await loadSettings()
})
</script>

<template>
  <div class="container">
    <div class="mx-auto py-12">
      <h1 class="mb-14 text-center text-xl text-content-100">
        {{ $t('nav.main.Admin') }}
      </h1>

      <OLoading
        v-if="isLoadingSettings"
        active
        icon-size="xl"
      />

      <FormGroup label="Settings" icon="settings" class="relative mx-auto w-1/2">
        <div class="mb-8 space-y-8">
          <OField v-for="(_, key) in settings" :key="key" :label="capitalize(key)">
            <OInput
              v-model="settings[key]"
              :placeholder="key"
              type="text"
              expanded
              size="lg"
            />
          </OField>
        </div>

        <div class="sticky bottom-0 flex items-center justify-center gap-4 bg-bg-main/50 py-4 backdrop-blur-sm">
          <OButton
            variant="primary"
            size="lg"
            outlined
            :label="$t('action.reset')"
            @click="loadSettings"
          />
          <ConfirmActionTooltip
            class="inline"
            :confirm-label="$t('action.ok')"
            title="Are you sure you want to remove the setting?"
            @confirm="onEditSettings(settings)"
          >
            <OButton
              variant="primary"
              size="lg"
              :loading="editingSetting"
              :label="$t('action.save')"
            />
          </ConfirmActionTooltip>
        </div>
      </FormGroup>
    </div>
  </div>
</template>
