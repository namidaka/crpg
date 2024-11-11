<script setup lang="ts">
import { getUserById } from '~/services/users-service'
import { moderationUserKey } from '~/symbols/moderator'

const props = defineProps<{ id: string }>()

definePage({
  meta: {
    roles: ['Moderator', 'Admin'],
  },
  props: true,
})

const { execute: loadUser, state: user } = await useAsyncState(
  () => getUserById(Number(props.id)),
  null,
  {
    resetOnExecute: false,
  },
)

provide(moderationUserKey, user)
</script>

<template>
  <div class="container">
    <div class="mb-14 flex items-center justify-center gap-8">
      <h1 class="text-lg text-content-100">
        <UserMedia
          :user="user!"
          size="xl"
          :clan="user?.clan"
        />
      </h1>

      <div class="flex items-center justify-center gap-2">
        <RouterLink
          v-slot="{ isExactActive }"
          :to="{ name: 'ModeratorUserIdInformation' }"
        >
          <OButton
            :variant="isExactActive ? 'transparent-active' : 'transparent'"
            size="lg"
            label="Information"
          />
        </RouterLink>

        <RouterLink
          v-slot="{ isExactActive }"
          :to="{ name: 'ModeratorUserIdRestrictions' }"
        >
          <OButton
            :variant="isExactActive ? 'transparent-active' : 'transparent'"
            size="lg"
            :label="$t('restriction.title')"
          />
        </RouterLink>

        <RouterLink
          v-slot="{ isExactActive }"
          :to="{ name: 'ModeratorUserIdActivityLogs' }"
        >
          <OButton
            :variant="isExactActive ? 'transparent-active' : 'transparent'"
            size="lg"
            label="Logs"
          />
        </RouterLink>
      </div>
    </div>

    <RouterView @update="loadUser" />
  </div>
</template>
