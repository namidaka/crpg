<script setup lang="ts">
import type { ActivityLog, ActivityLogType } from '~/models/activity-logs'
import type { UserPublic } from '~/models/user'

import { getItemImage } from '~/services/item-service'

withDefaults(
  defineProps<{
    activityLog: ActivityLog
    user: UserPublic
    users: Record<number, UserPublic>
    isSelfUser: boolean
  }>(),
  {
    users: () => ({}),
  },
)

const emit = defineEmits<{
  (e: 'addUser', id: number): void
  (e: 'addType', type: ActivityLogType): void
}>()
</script>

<template>
  <div
    class="flex-0 inline-flex w-auto flex-col space-y-2 rounded-lg bg-base-200 p-4"
    :class="[isSelfUser ? 'self-start' : 'self-end']"
  >
    <div class="flex items-center gap-2">
      <RouterLink
        :to="{ name: 'ModeratorUserIdRestrictions', params: { id: user.id } }"
        class="inline-block hover:text-content-100"
      >
        <UserMedia :user="user" />
      </RouterLink>

      <div class="text-2xs text-content-300">
        {{ $d(activityLog.createdAt, 'long') }}
      </div>

      <Tag
        variant="primary"
        :label="activityLog.type"
        data-aq-addLogItem-type
        @click="emit('addType', activityLog.type)"
      />
    </div>

    <i18n-t
      :keypath="`activityLog.tpl.${activityLog.type}`"
      tag="div"
      scope="global"
    >
      <template
        v-if="'price' in activityLog.metadata"
        #price
      >
        <Coin
          :value="Number(activityLog.metadata.price)"
          data-aq-addLogItem-tpl-goldPrice
        />
      </template>

      <template
        v-if="'gold' in activityLog.metadata"
        #gold
      >
        <Coin
          :value="Number(activityLog.metadata.gold)"
          data-aq-addLogItem-tpl-goldPrice
        />
      </template>

      <template
        v-if="'heirloomPoints' in activityLog.metadata"
        #heirloomPoints
      >
        <span
          class="inline-flex gap-1.5 align-text-bottom font-bold text-primary"
          data-aq-addLogItem-tpl-heirloomPoints
        >
          <OIcon
            icon="blacksmith"
            size="lg"
          />
          {{ $n(Number(activityLog.metadata.heirloomPoints)) }}
        </span>
      </template>

      <template
        v-if="'itemId' in activityLog.metadata"
        #itemId
      >
        <span
          class="inline"
          data-aq-addLogItem-tpl-itemId
        >
          <VTooltip
            placement="auto"
            class="inline-block"
          >
            <span class="font-bold text-content-100">{{ activityLog.metadata.itemId }}</span>
            <template #popper>
              <!-- TODO: need baseId (replace _h0?) -->
              <img
                :src="getItemImage(activityLog.metadata.itemId)"
                class="size-full object-contain"
              >
            </template>
          </VTooltip>
        </span>
      </template>

      <template
        v-if="'experience' in activityLog.metadata"
        #experience
      >
        <span
          class="font-bold text-content-100"
          data-aq-addLogItem-tpl-experience
        >
          {{ $n(Number(activityLog.metadata.experience)) }}
        </span>
      </template>

      <template
        v-if="'damage' in activityLog.metadata"
        #damage
      >
        <span
          class="font-bold text-status-danger"
          data-aq-addLogItem-tpl-damage
        >
          {{ $n(Number(activityLog.metadata.damage)) }}
        </span>
      </template>

      <template
        v-if="Number(activityLog.metadata.targetUserId) in users"
        #targetUserId
      >
        <div
          class="inline-flex items-center gap-1 align-middle"
          data-aq-addLogItem-tpl-targetUserId
        >
          <RouterLink
            :to="{
              name: 'ModeratorUserIdRestrictions',
              params: { id: activityLog.metadata.targetUserId },
            }"
            class="inline-block hover:text-content-100"
            target="_blank"
          >
            <UserMedia :user="users[Number(activityLog.metadata.targetUserId)]" />
          </RouterLink>
          <OButton
            v-if="isSelfUser"
            size="2xs"
            icon-left="add"
            rounded
            variant="secondary"
            data-aq-addLogItem-addUser-btn
            @click="emit('addUser', Number(activityLog.metadata.targetUserId))"
          />
        </div>
      </template>

      <template
        v-if="'actorUserId' in activityLog.metadata"
        #actorUserId
      >
        <div class="inline-flex items-center gap-1 align-middle">
          <RouterLink
            class="inline-block hover:text-content-100"
            :to="{
              name: 'ModeratorUserIdInformation',
              params: { id: activityLog.metadata.actorUserId },
            }"
            target="_blank"
          >
            <UserMedia
              :user="users[Number(activityLog.metadata.actorUserId)]"
              hidden-clan
              hidden-platform
            />
          </RouterLink>
        </div>
      </template>

      <template
        v-if="'instance' in activityLog.metadata"
        #instance
      >
        <Tag
          variant="info"
          :label="activityLog.metadata.instance"
        />
      </template>

      <template
        v-if="'gameMode' in activityLog.metadata"
        #gameMode
      >
        <Tag
          variant="info"
          :label="activityLog.metadata.gameMode"
        />
      </template>

      <template
        v-if="'oldName' in activityLog.metadata"
        #oldName
      >
        <span class="font-bold text-content-100">{{ activityLog.metadata.oldName }}</span>
      </template>

      <template
        v-if="'newName' in activityLog.metadata"
        #newName
      >
        <span class="font-bold text-content-100">{{ activityLog.metadata.newName }}</span>
      </template>

      <template
        v-if="'characterId' in activityLog.metadata"
        #characterId
      >
        <span class="font-bold text-content-100">{{ activityLog.metadata.characterId }}</span>
      </template>

      <template
        v-if="'generation' in activityLog.metadata"
        #generation
      >
        <span class="font-bold text-content-100">{{ activityLog.metadata.generation }}</span>
      </template>

      <template
        v-if="'level' in activityLog.metadata"
        #level
      >
        <span class="font-bold text-content-100">{{ activityLog.metadata.level }}</span>
      </template>

      <template
        v-if="'message' in activityLog.metadata"
        #message
      >
        <span class="font-bold text-content-100">{{ activityLog.metadata.message }}</span>
      </template>

      <template
        v-if="'clanId' in activityLog.metadata"
        #clanId
      >
        <span class="font-bold text-content-100">{{ activityLog.metadata.clanId }}</span>
      </template>

      <template
        v-if="'userItemId' in activityLog.metadata"
        #userItemId
      >
        <span class="font-bold text-content-100">{{ activityLog.metadata.userItemId }}</span>
      </template>
    </i18n-t>
  </div>
</template>
