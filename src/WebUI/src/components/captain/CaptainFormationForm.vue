
import { useUserStore } from '@/stores/user';

import charactersVue from '@/pages/characters.vue';

import { useUserStore } from '@/stores/user';

import { useUserStore } from '@/stores/user';

import { useUserStore } from '@/stores/user';

import { getFormations } from '@/services/captain-service';

import { getFormations } from '@/services/captain-service';
<script setup lang="ts">
import { useVuelidate } from '@vuelidate/core';
import { useUserStore } from '@/stores/user';
import { notify, NotificationType } from '@/services/notification-service';
import { t } from '@/services/translate-service';

const props = withDefaults(
  defineProps<{
    formation: CaptainFormation | null,
    id: number | null,
    characterid: number | null,
    weight: 1,
  }>(),
  {
    formation: null,
    id: null,
    characterid: null,
    weight: 1,
  }
);

const userStore = useUserStore();
if (userStore.characters.length === 0) {
    await userStore.fetchCharacters();
}

const formationCharacter = userStore.characters.find(c => c.id == props.formation.characterId);

const { user } = toRefs(useUserStore());
const router = useRouter();

const emit = defineEmits<{
  (e: 'change', characterid: number, id: number, weight: number): void;
}>();

const onFormationChange = (characterid: number, id: number, weight: number) => {
    emit('change', characterid, id, weight);
}

</script>

<template>
    <div class="mb-8 space-y-4">
        <div class="order-1 flex items-center gap-4">
        <VDropdown :triggers="['click']" placement="bottom-end">
          <template #default="{ shown }">
            <OButton variant="primary" outlined size="lg">
                <CharacterMedia
                :character="formationCharacter"
                :isActive="false"
              />
              <div class="h-4 w-px select-none bg-border-300"></div>

              <OIcon
                icon="chevron-down"
                size="lg"
                :rotation="shown ? 180 : 0"
                class="text-content-400"
              />
            </OButton>
          </template>

          <template #popper="{ hide }">
            <div class="min-w-[24rem]">
              <DropdownItem
                class="justify-between"
                @click="hide"
              >
                
              </DropdownItem>

            </div>
          </template>
        </VDropdown>
    </div>
    </div>
</template>
