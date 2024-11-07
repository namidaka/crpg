<script setup lang="ts">
import { useVuelidate } from '@vuelidate/core'

import type { Character } from '~/models/character'

import { maxLength, minLength, required } from '~/services/validators-service'

const props = defineProps<{
  character: Character
}>()

const emit = defineEmits<{
  (e: 'cancel'): void
  (e: 'confirm', { name }: { name: string }): void
}>()

const nameModel = ref<string>(props.character.name)
const $v = useVuelidate(
  {
    nameModel: {
      maxLength: maxLength(32),
      minLength: minLength(2),
      required,
    },
  },
  { nameModel },
)

const onCancel = () => {
  $v.value.$reset()
  emit('cancel')
}

const onConfirm = async () => {
  if (!(await $v.value.$validate())) { return }

  emit('confirm', { name: nameModel.value })
}

const wasChange = computed(() => nameModel.value !== props.character.name)
</script>

<template>
  <div class="space-y-14">
    <h4 class="text-center text-xl">
      {{ $t('character.settings.update.title') }}
    </h4>

    <div class="space-y-8">
      <OField
        :label="$t('character.settings.update.form.field.name')"
        v-bind="{
          ...($v.nameModel.$error && {
            variant: 'danger',
            message: $v.$errors[0].$message,
          }),
        }"
      >
        <OInput
          v-model="nameModel"
          size="lg"
          class="w-full"
          :maxlength="32"
          counter
          @blur="$v.$touch"
          @focus="$v.$reset"
        />
      </OField>
    </div>

    <div class="flex items-center justify-center gap-4">
      <OButton
        variant="primary"
        outlined
        size="xl"
        :label="$t('action.cancel')"
        @click="onCancel"
      />
      <OButton
        :disabled="$v.nameModel.$invalid || !wasChange"
        variant="primary"
        size="xl"
        :label="$t('action.save')"
        @click="onConfirm"
      />
    </div>
  </div>
</template>
