<script setup lang="ts">
import { useUserStore } from '@/stores/user';
import { watch } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import {
  required,
  integer,
} from '@/services/validators-service';
import { notify } from '@/services/notification-service';
import { Captain, CaptainFormation } from '@/models/captain'
import {
  getCaptain,
  getFormations,
  assignCharacterToFormation,
  setFormationWeight,
} from '@/services/captain-service';
import { currentLocale, t } from '@/services/translate-service';
definePage({
  meta: {
    layout: 'default',
    bg: 'background-4.webp',
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const router = useRouter();
const selfCaptain = await getCaptain();
const selfFormations = ref(await getFormations());

const sliders = [
  { slider: 1, locked: false },
  { slider: 2, locked: false },
  { slider: 3, locked: false },
]

const selectedFormations = computed(() => {
  return [1, 2, 3].map(id => selfFormations.value.find(f => f.number === id)).filter(f => f !== undefined);
});

const totalWeight = computed(() => {
  return selfFormations.value
  .filter(f => f.characterId !== null)
  .reduce((acc, curr) => acc + curr.weight, 0);
});


const onFormationChange = (updatedFormation: CaptainFormation) => {
  const index = selfFormations.value.findIndex(f => f.number === updatedFormation.number);
  if (index !== -1) {
    // Update the formation in place
    selfFormations.value[index] = updatedFormation;
  } else {
    // Optionally handle the case where the formation isn't found,
    // such as adding the new formation to the array
    selfFormations.value.push(updatedFormation);
  }
}

const lockFormationWeight = (updatedFormation: number) => {
  sliders.find(s => s.slider == updatedFormation)?.locked == true ? false : true;
}


const assignFormationWeight = async () => {
  $v.value.$touch(); // Mark the fields as touched to show errors
  
  if ($v.value.$invalid) {
    notify('Validation failed. Total weight must be 100.');
    return; // Stop if validation fails
  }
  
  for (const formation of selectedFormations.value){
    if (formation){
      await setFormationWeight(formation.number, formation.weight);
    }
  }
    
    notify(t('captain.formation.weight.notify.success'));
};

const mustBeMaxWeight = (value: number) => value == 100

const $v = useVuelidate(
  {
    totalWeight: {
      required,
      value: mustBeMaxWeight,
    },
  },
  { totalWeight }
);

</script>

<template>
  <div class="container relative py-6">
    <div class="mb-5 flex justify-center">
      <OIcon icon="trumpet" size="5x" class="text-more-support" />
    </div>
    <Heading :title="$t('captain.title')" />
    <div class="sticky bottom-4 left-0 z-10 flex w-full justify-center rounded-lg p-4">
      <div class="inline-flex gap-1.5 align-middle">
          Total value:
          <SvgSpriteImg name="coin" viewBox="0 0 18 18" class="w-4" />
          <span class="text-xs font-bold text-primary">{{ $n(10000) }}</span>
        </div>
    </div>
    <div class="relative grid grid-cols-12 gap-5">
      <div v-for="(formation, index) in selectedFormations" :key="index" class="col-span-4">
        <CaptainFormationForm :formation="formation" @update:formation="onFormationChange" @toggleLock="lockFormationWeight" />
      </div>
    </div>
    <div class="flex justify-center">
      <div v-if="$v.$error" class="text-danger">
        Total weight must be 100
    </div></div>


    <div class="flex justify-center p-10">
      <OButton
          variant="primary"
          size="xl"
          :label="$t('action.apply')"
          @click="assignFormationWeight()"
        />
    </div>

  </div>
</template>

<style lang="css">

</style>