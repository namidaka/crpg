<script setup lang="ts">
import { ref, watch } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { useUserStore } from '@/stores/user';
import { assignCharacterToFormation, setFormationWeight } from '@/services/captain-service'
import type { CaptainFormation } from '@/models/captain'
import type { Character } from '@/models/character';
import { notify, NotificationType } from '@/services/notification-service';
import { t } from '@/services/translate-service';

const props = withDefaults(
  defineProps<{
    formation: CaptainFormation | null,
  }>(),
  {
    formation: null,
  }
);

const formationCharacter = ref<Character | null>(null);
const userStore = useUserStore();

onMounted(async () => {
  if (userStore.characters.length === 0) {
    await userStore.fetchCharacters();
  }
});

watch(
  () => props.formation?.characterId,
  (newCharacterId) => {
    formationCharacter.value = userStore.characters.find(c => c.id === newCharacterId) || null;
  },
  {
    immediate: true,
  }
);

const setFormationCharacter = async (characterId: number, active: boolean) => {
    await assignCharacterToFormation(props.formation!.id, characterId, active);
    emit('update:formation', { characterId, id: props.formation!.id, weight: props.formation!.weight});
    notify(t('captain.formation.character.notify.success'));
}

const assignFormationWeight = async () => {
    await setFormationWeight(props.formation!.id, props.formation!.weight);
    notify(t('captain.formation.weight.notify.success'));
}

const route = useRoute('Captain');
const { user } = toRefs(useUserStore());
const router = useRouter();

const emit = defineEmits<{
  (e: 'update:formation', formation: CaptainFormation): void;
}>();

const onFormationChange = (characterid: number, id: number, weight: number) => {
    emit('update:formation', { characterId: characterid, id, weight });
}

</script>

<template>
    <div class="mx-auto max-w-lg rounded-xl border border-border-200 p-6 bg-base-100 opacity-75">
        <div class="mb-8 space-y-4">
            <div class="prose prose-invert px-12 pb-6 text-center">
                <h2 class="text-m text-content-100">{{ $t('captain.formation.title', { id: formation!.id }) }}</h2>
                <Divider />
            </div>
        <div class="text-center text">
            {{ $t('captain.formation.character.title') }}
        </div>
                
            <div class="flex justify-center pb-6">
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
                        <div class="min-w-[18rem]">
                            <DropdownItem
                                v-for="char in userStore.characters"
                                :checked="char.id === formation?.characterId"
                                tag="RouterLink"
                                :to="{ name: route.name }"
                                class="justify-between"
                                @click="() => { setFormationCharacter(char.id, true); hide(); }"
                            >
                            <CharacterMedia :character="char" />
                            </DropdownItem>
                            <DropdownItem
                                class="text-primary hover:text-primary-hover"
                                @click="
                                () => {
                                    setFormationCharacter(0, false);
                                    hide();
                                }
                                "
                            >
                                <OIcon icon="minus" size="lg" />
                                {{ $t('captain.formation.character.remove') }}
                            </DropdownItem>
                        </div>
                    </template>
                </VDropdown>

            </div>
            <Divider />
        </div>    
        <div class="text-center text-l pb-6">
            {{ $t('captain.formation.weight.title') }}
        </div>       
        <div class="pb-12">
            <VueSlider
                    v-model="formation!.weight"
                    :min="0"
                    :max="100"
                    :step="1"
                    :marks="[0, 50, 100]"
                />

        </div>
        <div class="flex justify-center">
        <OButton
            variant="primary"
            size="xl"
            :label="$t('action.confirm')"
            @click="assignFormationWeight()"
          />
    </div>
</div>
</template>
