<script setup lang="ts">
import { SettingDataType } from '~/models/setting'

const {
  dataType = SettingDataType.String,
} = defineProps<{ dataType: SettingDataType }>()

const modelValue = defineModel<string>()
</script>

<template>
  <template v-if="dataType === SettingDataType.String">
    <OInput
      v-model="modelValue"
      placeholder="Setting value"
      size="lg"
      expanded
    />
  </template>
  <template v-if="dataType === SettingDataType.Number">
    <OInput
      v-model="modelValue"
      placeholder="Setting value"
      size="lg"
      expanded
      type="number"
    />
  </template>
  <template v-if="dataType === SettingDataType.Boolean">
    <OSwitch
      v-model="modelValue" label="Enabled"
      true-value="true"
      false-value="false"
    />
  </template>
  <template v-if="dataType === SettingDataType.Json">
    <OInput
      v-model="modelValue"
      placeholder="Setting value"
      size="lg"
      expanded
      type="textarea"
      rows="4"
    />
  </template>
</template>
