<script setup lang="ts">
import { ref, watch } from 'vue';
import { useUserStore } from '@/stores/user';
import { getCharacterItems, computeOverallPrice } from '@/services/characters-service';
import type { CaptainFormation } from '@/models/captain'
import type { Character } from '@/models/character';

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

if (userStore.characters.length === 0) {
    await userStore.fetchCharacters();
}

const route = useRoute('Captain');

const emit = defineEmits<{
  (e: 'update:formation', formation: CaptainFormation): void,
}>();

const setFormationCharacter = async (characterId: number | null) => {
    emit('update:formation', { characterId, number: props.formation!.number, weight: props.formation!.weight});
}

const onWeightChanged = (newWeight: number) => {
    emit('update:formation', { characterId: props.formation!.characterId ?? null, number: props.formation!.number, weight: newWeight });
}

watch(
  () => props.formation?.characterId,
  (newCharacterId) => {
    formationCharacter.value = userStore.characters.find(c => c.id === newCharacterId) || null;
  },
  {
    immediate: true,
  }
);

</script>

<template>
    <div class="mx-auto max-w-lg rounded-xl border border-border-200 p-6 bg-base-100 opacity-75">
        <div class="mb-8 space-y-4">
            <div class="prose prose-invert px-12 pb-6 text-center">
                <h2 class="text-m text-content-100">{{ $t('captain.formation.title', { id: formation!.number }) }}</h2>
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
                                @click="() => { setFormationCharacter(char.id); hide(); }"
                            >
                            <CharacterMedia :character="char" />
                            </DropdownItem>
                            <DropdownItem
                                class="text-status-danger hover:text-status-danger-hover"
                                @click="
                                () => {
                                    setFormationCharacter(null);
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
        <div class="flex items-center justify-center gap-3 pb-6">
            {{ $t('captain.formation.weight.title') }}
        </div>       
        <div class="pb-12">
            
            <VueSlider
                    v-model="formation!.weight"
                    :min="0"
                    :max="100"
                    :step="1"
                    :marks="[0, 50, 100]"
                    @change="(newValue: number) => onWeightChanged(newValue)"
                    class=""
                />

        </div>
</div>
</template>
