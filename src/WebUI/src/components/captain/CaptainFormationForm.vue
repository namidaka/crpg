
import { minLength } from '@vuelidate/validators';

import { getFormations } from '@/services/captain-service';

import { assignCharacterToFormation } from '@/services/captain-service';

import { number } from 'echarts';
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

const setFormationCharacter = async (characterid: number, status: boolean) => {
    await assignCharacterToFormation(props.formation.id);
}

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
    <div class="mx-auto max-w-lg rounded-xl border border-border-200 p-6 bg-base-100 opacity-75">
    <div class="mb-8 space-y-4">
        <div class="prose prose-invert px-12 pb-6 text-center">
            <h4 class="text-sm text-content-100">{{ $t('captain.formation.title', { id: formation.id }) }}</h4>
            <Divider />
        </div>
        
        <div class="flex justify-center">
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
                            v-for="char in userStore.characters"
                            :checked="char.id === formation.characterId"
                            class="justify-between"
                            @click="hide"
                        >
                            <CharacterSelectItem
                            :character="char"
                            :modelValue="formation.characterId === char.id"
                            @update:modelValue="(val: boolean) => onActivateCharacter(char.id, val)"
                            />
                        </DropdownItem>
                    </div>
                </template>
            </VDropdown>
            <VueSlider
                v-model="desiredWeight"
                :min="0"
                :max="100"
                :step="1"
                :marks="[min, max]"
            />
        </div>
    </div></div>
</template>
