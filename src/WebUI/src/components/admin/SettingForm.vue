<script setup lang="ts">
import { useVuelidate } from '@vuelidate/core'

import { type Setting, SettingDataType, type SettingEdition } from '~/models/setting'
import { errorMessagesToString, required } from '~/services/validators-service'

const {
  settingId = null,
  setting = null,
} = defineProps<{
  setting: Setting | null
  settingId: number | null
}>()

const emit = defineEmits<{
  submit: [setting: SettingEdition]
  delete: []
  cancel: []
}>()

const getDefaultSetting = (): SettingEdition => ({
  key: '',
  value: '',
  description: '',
  private: true,
  dataType: SettingDataType.String,
})

const settingModel = ref<SettingEdition>({ ...(setting ?? getDefaultSetting()) })

const $v = useVuelidate(
  {
    key: { required },
    value: { required },
  },
  settingModel,
)

watch(() => settingModel.value.dataType, () => {
  settingModel.value.value = ''
})

const submit = async () => {
  // TODO:
  if (!(await $v.value.$validate())) {
    return
  }

  emit('submit', settingModel.value)
}
</script>

<template>
  <div
    class="space-y-8"
  >
    <!-- TODO: ScrollableContainerComponent -->
    <div class="max-h-[66vh] space-y-8 overflow-auto">
      <OField>
        <OField
          class="flex-1"
          label="Key"
          v-bind="{
            ...($v.key.$error && {
              variant: 'danger',
              message: errorMessagesToString($v.key.$errors),
            }),
          }"
        >
          <OInput
            v-model="settingModel.key"
            placeholder="Setting key"
            expanded
            size="lg"
            required
            rows="3"
            @focus="$v.$reset"
          />
        </OField>

        <OField label="Data type">
          <VDropdown :triggers="['click']">
            <template #default="{ shown }">
              <OButton
                :label="settingModel.dataType"
                variant="secondary"
                size="lg"
                :icon-right="shown ? 'chevron-up' : 'chevron-down'"
              />
            </template>

            <template #popper="{ hide }">
              <DropdownItem
                v-for="sdt in Object.keys(SettingDataType)"
                :key="sdt"
                class="min-w-60 max-w-xs"
              >
                <ORadio
                  v-model="settingModel.dataType"
                  :native-value="sdt"
                  @change="hide"
                >
                  {{ sdt }}
                </ORadio>
              </DropdownItem>
            </template>
          </VDropdown>
        </OField>
      </OField>

      <OField
        label="Value"
        v-bind="{
          ...($v.value.$error && {
            variant: 'danger',
            message: errorMessagesToString($v.value.$errors),
          }),
        }"
      >
        <SettingValue
          v-model="settingModel.value"
          :data-type="settingModel.dataType"
          @focus="$v.$reset"
        />
      </OField>

      <OField label="Description">
        <OInput
          v-model="settingModel.description"
          placeholder="Setting description"
          size="lg"
          expanded
          type="textarea"
          rows="3"
        />
      </OField>

      <OField label="Access">
        <OSwitch v-model="settingModel.private" label="Private setting" />
      </OField>
    </div>

    <div class="flex items-center justify-center gap-4">
      <OButton
        variant="primary"
        size="lg"
        outlined
        :label="$t('action.cancel')"
        @click="$emit('cancel')"
      />

      <OButton
        variant="primary"
        size="lg"
        :label="$t('action.save')"
        @click="submit"
      />
    </div>

    <div v-if="settingId" class="text-center">
      You can
      <ConfirmActionTooltip
        class="inline"
        :confirm-label="$t('action.ok')"
        title="Are you sure you want to remove the setting?"
        @confirm="$emit('delete')"
      >
        <span class="cursor-pointer text-status-danger hover:text-opacity-80">delete setting</span>
      </ConfirmActionTooltip>.
      You won't be able to restore it
    </div>
  </div>
</template>
