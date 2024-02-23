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
const selfFormations = await getFormations();

const onFormationChanged = async (formation: CaptainFormation) => {
  const clan = await assignCharacterToFormation(formation.id);
  notify(t('clan.create.notify.success'));
};
</script>

<template>
  <div class="container relative py-6">
    <div class="mb-5 flex justify-center">
      <OIcon icon="trumpet" size="5x" class="text-more-support" />
    </div>
    <Heading :title="$t('captain.title')" />
    <div class="relative grid grid-cols-12 gap-5">
      <div class="col-span-4">
        <CaptainFormationForm :formation="selfFormations.find(f => f.id == 1)" @change="onFormationChanged" />
      </div>
      <div class="col-span-4">
        <CaptainFormationForm :formation="selfFormations.find(f => f.id == 2)" @change="onFormationChanged" />
      </div>
      <div class="col-span-4">
        <CaptainFormationForm :formation="selfFormations.find(f => f.id == 3)" @change="onFormationChanged" />
      </div>
    </div>
  </div>
</template>

<style lang="css">

</style>