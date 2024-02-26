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
import { not } from '@vuelidate/validators';
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
let haveFormationsChanged = ref(false);


const selectedFormations = computed(() => {
  return [1, 2, 3].map(id => selfFormations.value.find(f => f.number === id)).filter(f => f !== undefined);
});

const onFormationChange = (updatedFormation: CaptainFormation) => {
  haveFormationsChanged.value = true;
  const index = selfFormations.value.findIndex(f => f!.number === updatedFormation.number);
  if (index !== -1) {
    selfFormations.value.splice(index, 1, updatedFormation);
  }
};


watch(selectedFormations, (newFormations) => {
  
}, { deep: true });

const applyFormationChanges = async () => { 
  for (const formation of selectedFormations.value){
    if (formation){
      await setFormationWeight(formation.number, formation.weight);
      await assignCharacterToFormation(formation.number, formation.characterId);
    }
  }
  notify(t('captain.formation.update.notify.success'));
  haveFormationsChanged.value = false;
};

</script>

<template>
  <div class="container relative py-6">
    <div class="mb-5 flex justify-center">
      <OIcon icon="trumpet" size="5x" class="text-more-support" />
    </div>
    <Heading :title="$t('captain.title')" />
    <div class="relative grid grid-cols-12 gap-5">
      <div v-for="(formation, index) in selectedFormations" :key="index" class="col-span-4">
        <CaptainFormationForm :formation="formation" @update:formation="onFormationChange" />
      </div>
    </div>


    <div class="flex justify-center p-10">
      <OButton
          variant="primary"
          size="xl"
          :label="$t('action.apply')"
          @click="applyFormationChanges"
          :disabled="!haveFormationsChanged"
        />
    </div>

  </div>
</template>

<style lang="css">

</style>