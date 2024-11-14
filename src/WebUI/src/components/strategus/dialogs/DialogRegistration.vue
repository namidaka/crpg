<script setup lang="ts">
import { registerUser } from '~/services/strategus-service'
import { useUserStore } from '~/stores/user'

const emit = defineEmits<{
  registered: []
}>()

const { user } = toRefs(useUserStore())

const start = async () => {
  await registerUser()
  emit('registered')
}
</script>

<template>
  <DialogBase>
    <div class="prose prose-invert">
      <h2>
        {{ $t('strategus.registration.welcome') }}
      </h2>

      <Divider />

      <p>
        {{ $t('strategus.registration.description') }}
      </p>

      <i18n-t
        scope="global"
        keypath="strategus.registration.join"
        tag="p"
      >
        <template #region>
          <strong>{{ $t(`region.${user!.region}`) }}</strong>
        </template>
      </i18n-t>

      <OButton
        variant="primary"
        size="xl"
        label="Start"
        @click="start"
      />

      <Divider />

      <p>
        {{ $t('strategus.registration.hint') }}
      </p>
    </div>
  </DialogBase>
</template>
