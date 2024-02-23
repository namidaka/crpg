<script setup lang="ts">
import { useUserStore } from '@/stores/user';
import { Captain, CaptainFormation } from '@/models/captain'
import {
  getCaptain,
  getFormations,
  assignCharacterToFormation,
} from '@/services/captain-service';

definePage({
  meta: {
    layout: 'default',
    roles: ['User', 'Moderator', 'Admin'],
  },
});


</script>

<template>
  <div class="container relative py-6">
    Hi
    <div class="order-1 flex items-center gap-4">
      Hello
    </div>
  </div>
</template>

<style lang="css">

</style>