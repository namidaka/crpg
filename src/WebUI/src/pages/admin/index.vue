<script setup lang="ts">
import { useToggle } from '@vueuse/core'

import type { Setting, SettingEdition } from '~/models/setting'

import { deleteSetting, getSettings, setSetting } from '~/services/settings-service'
import { useAsyncCallback } from '~/utils/useAsyncCallback'

definePage({
  meta: {
    roles: ['Admin'],
  },
})

const { execute: loadSettings, state: settings } = useAsyncState(
  () => getSettings(),
  [],
)

const { execute: onSetSetting, loading: editingSetting } = useAsyncCallback(async (setting: SettingEdition) => {
  await setSetting(setting)
  await loadSettings()
})

const { execute: onDeleteSetting, loading: deletingSetting } = useAsyncCallback(async (id: number) => {
  await deleteSetting(id)
  await loadSettings()
})

const onClickSetting = (setting: Setting) => {
  selectedSetting.value = setting
}

const selectedSetting = ref<Setting | null>(null)
const [shownCreateSettingDialog, toggleCreateSettingDialog] = useToggle()
</script>

<template>
  <div class="container">
    <div class="mx-auto py-12">
      <h1 class="mb-14 text-center text-xl text-content-100">
        {{ $t('nav.main.Admin') }}
      </h1>

      <div>
        <div class="mb-8 flex items-center gap-4">
          <h2 class="text-lg">
            Settings
          </h2>

          <OButton
            native-type="submit"
            variant="primary"
            size="sm"
            label="Add setting"
            @click="toggleCreateSettingDialog"
          />
        </div>

        <SettingsTable
          :settings="settings"
          :loading="editingSetting || deletingSetting"
          @open-setting="onClickSetting"
        />
      </div>
    </div>

    <Modal
      closable
      :auto-hide="false"
      :shown="shownCreateSettingDialog || Boolean(selectedSetting)"
      @hide="() => {
        selectedSetting = null
        toggleCreateSettingDialog(false)
      }"
    >
      <template #popper="{ hide }">
        <div class="w-[48rem] space-y-6 p-6">
          <div class="pb-4 text-center text-xl text-content-100">
            Setting
          </div>

          <SettingForm
            :setting-id="selectedSetting?.id || null"
            :setting="selectedSetting"
            @submit="(setting) => {
              hide();
              onSetSetting(setting);
            }"
            @delete="() => {
              hide();
              onDeleteSetting(selectedSetting!.id)
            }"
            @cancel="hide"
          />
        </div>
      </template>
    </Modal>
  </div>
</template>
