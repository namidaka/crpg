<script setup lang="ts">
import { ref, watch } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { useUserStore } from '@/stores/user';
import {
  characterKey,
  characterCharacteristicsKey,
  characterHealthPointsKey,
  characterItemsKey,
  characterItemsStatsKey,
  equippedItemsBySlotKey,
} from '@/symbols/character';
import { getCaptain, assignCharacterToFormation, setFormationWeight } from '@/services/captain-service'
import { getCharacterItems, computeOverallPrice } from '@/services/characters-service';
import type { CaptainFormation } from '@/models/captain'
import type { Character, CharacterOverallItemsStats } from '@/models/character';
import { notify, NotificationType } from '@/services/notification-service';
import { t } from '@/services/translate-service';

import { usePollInterval } from '@/composables/use-poll-interval';

const props = withDefaults(
  defineProps<{
    formation: CaptainFormation | null,
  }>(),
  {
    formation: null,
  }
);

const loadCharacterItemsSymbol = Symbol('loadCharacterItems');
const loadCharactersSymbol = Symbol('fetchCharacters');
const loadUserItemsSymbol = Symbol('fetchUserItems');

const { subscribe, unsubscribe } = usePollInterval();
const formationCharacter = ref<Character | null>(null);
const userStore = useUserStore();

if (userStore.characters.length === 0) {
    await userStore.fetchCharacters();
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

onMounted(() => {
  subscribe(loadCharacterItemsSymbol, () => loadCharacterItems(0, { id: formationCharacter.value?.id }));
  subscribe(loadUserItemsSymbol, userStore.fetchUserItems);
});

const { state: characterItems, execute: loadCharacterItems } = useAsyncState(
  ({ id }: { id: number | null | undefined }) => {
    if (id !== null && id !== undefined) {
      return getCharacterItems(id);
    }
    return Promise.resolve([]);
  },
  [],
  {
    immediate: false,
    resetOnExecute: false,
  }
);

const itemsStats = computed((): number => {
  const items = characterItems.value.map(ei => ei.userItem.item);
  return computeOverallPrice(items);
});

const setFormationCharacter = async (characterId: number, active: boolean) => {
    await assignCharacterToFormation(props.formation!.number, characterId, active);
    emit('update:formation', { characterId, number: props.formation!.number, weight: props.formation!.weight});
    notify(t('captain.formation.character.notify.success'));
}

const route = useRoute('Captain');
const { user } = toRefs(useUserStore());
const router = useRouter();
let locked = ref(false);

const emit = defineEmits<{
  (e: 'update:formation', formation: CaptainFormation): void,
  (e: 'toggleLock', number: number): void;
}>();

const onFormationChange = (formation: CaptainFormation) => {
    emit('update:formation', { characterId: formation.characterId ?? null, number: formation.number, weight: formation.weight });
}

const toggleLock = (number: number) => {
    locked.value = !locked.value;
    emit('toggleLock',  number);
}

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
                                @click="() => { setFormationCharacter(char.id, true); hide(); }"
                            >
                            <CharacterMedia :character="char" />
                            </DropdownItem>
                            <DropdownItem
                                class="text-status-danger hover:text-status-danger-hover"
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
        <div class="flex items-center justify-center gap-3 pb-6">
            {{ $t('captain.formation.weight.title') }}
            <OIcon icon="lock" size="s" class="hover:text-content-400" v-tooltip="$t(`captain.formation.weight.lock`)" @click="toggleLock"/>
        </div>       
        <div class="pb-12">
            
            <VueSlider
                    :disabled="locked"
                    v-model="formation!.weight"
                    :min="0"
                    :max="100"
                    :step="1"
                    :marks="[0, 50, 100]"
                    @change="onFormationChange(formation!)"
                    class=""
                />

        </div>
</div>
</template>
