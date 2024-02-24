<script setup lang="ts">
import { useUserStore } from '@/stores/user';
import { notify } from '@/services/notification-service';
import { Captain, CaptainFormation } from '@/models/captain'
import {
  getCaptain,
  getFormations,
  assignCharacterToFormation,
} from '@/services/captain-service';
import { t } from '@/services/translate-service';
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

const selectedFormations = computed(() => {
  return [1, 2, 3].map(id => selfFormations.value.find(f => f.id === id)).filter(f => f !== undefined);
});

const onFormationChanged = async (formation: CaptainFormation) => {
  const index = selfFormations.value.findIndex(f => f.id === formation.id);
  if (index !== -1) {
    selfFormations.value[index] = formation;
  } else {
    selfFormations.value.push(formation);
  }
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
        <CaptainFormationForm :formation="formation" @update:formation="onFormationChanged" />
      </div>
    </div>
  </div>
</template>

<style lang="css">

</style>