<script setup lang="ts">
import { characterClassToIcon } from '@/services/characters-service';
import { CharacterClass } from '@/models/character';

defineEmits<{
  startOnboarding: [];
}>();

// TODO: Api
const presets = ref([
  {
    id: 1,
    gold: 200000,
    class: CharacterClass.Peasant,
    level: 0,
    description: 'The most interesting start. Start with a naked ass, end with a tin ass',
  },
  {
    id: 2,
    gold: 30000,
    class: CharacterClass.Infantry,
    level: 30,
    description: "That's the base. A baseline",
  },
  {
    id: 3,
    gold: 30000,
    class: CharacterClass.ShockInfantry,
    level: 30,
    description: 'Kuyak enjoyer',
  },
  {
    id: 4,
    gold: 1,
    class: CharacterClass.Archer,
    level: 30,
    description: 'Do not choose this start under any conditions',
  },
  {
    id: 5,
    gold: 100000,
    class: CharacterClass.Cavalry,
    level: 30,
    description: 'Mount and blade',
  },
  {
    id: 6,
    gold: 100000,
    class: CharacterClass.MountedArcher,
    level: 30,
    description: '( ͡° ͜ʖ ͡°)',
  },
]);

const presetModel = ref<number | null>(null);
</script>

<template>
  <Modal closable shown>
    <template #popper>
      <div class="flex max-h-[90vh] w-[40rem] flex-col">
        <header class="relative min-h-[10rem]">
          <!-- TODO: poster -->
          <img
            class="absolute inset-0 aspect-video h-full w-full object-cover opacity-50"
            :src="`/images/bg/background-1.webp`"
          />
          <!-- TODO: heading cmp from clan-armory branch -->
          <div class="absolute left-1/2 top-1/2 w-full -translate-x-1/2 -translate-y-1/2 space-y-2">
            <div class="flex justify-center">
              <SvgSpriteImg name="logo" viewBox="0 0 162 124" class="w-16" />
            </div>
            <div class="flex select-none items-center justify-center gap-8 text-center">
              <SvgSpriteImg
                name="logo-decor"
                viewBox="0 0 108 10"
                class="w-24 rotate-180 transform"
              />
              <h2 class="text-2xl text-white">Welcome warrior</h2>
              <SvgSpriteImg name="logo-decor" viewBox="0 0 108 10" class="w-24" />
            </div>
          </div>
        </header>

        <div class="h-full space-y-10 overflow-y-auto px-12 py-8">
          <div class="prose prose-invert">
            <p>
              Lorem ipsum dolor sit amet consectetur adipisicing elit. Dicta magni, suscipit,
              facilis sapiente minus voluptate laborum nemo placeat eius totam harum ipsam?
            </p>
          </div>

          <div class="space-y-4">
            <p class="text-center">Pick your start and go kick some ass:</p>

            <div class="grid grid-cols-2 gap-4">
              <div
                v-for="preset in presets"
                class="relative cursor-pointer rounded-xl border border-border-200 p-3 pr-11 hover:ring hover:ring-primary"
                @click="presetModel = preset.id"
              >
                <div class="space-y-2">
                  <div class="flex items-center gap-2">
                    <OIcon :icon="characterClassToIcon[preset.class]" size="lg" />
                    {{ $t(`character.class.${preset.class}`) }}
                  </div>

                  <div class="flex items-center gap-2">
                    {{ preset.level }}
                    lvl,
                    <Coin :value="preset.gold" v-tooltip.bottom="$t('user.field.gold')" />

                    <div class="flex items-center gap-2">
                      <!-- <span class="text-lg font-semibold text-status-success">+</span> -->
                    </div>
                  </div>

                  <p class="text-2xs text-content-400">{{ preset.description }}</p>
                </div>

                <div class="absolute right-3 top-3">
                  <ORadio v-model="presetModel" :native-value="preset.id" />
                </div>
              </div>
            </div>
          </div>

          <Divider />

          <div class="space-y-4">
            <p class="text-center">Lorem ipsum dolor sit amet consectetur.</p>

            <div class="flex justify-center">
              <OButton
                variant="primary"
                outlined
                size="xl"
                iconLeft="tag"
                :label="`Start onboarding`"
                @click="$emit('startOnboarding')"
              />
            </div>
          </div>

          <div class="space-y-6">
            <FormGroup icon="help-circle" label="Helpful links" collapsed>
              <div>TODO:</div>
            </FormGroup>

            <FormGroup icon="settings" label="Some interesting" collapsed>
              <div>TODO:</div>
            </FormGroup>
          </div>
        </div>

        <footer>
          <Divider />

          <div class="px-12 py-6">
            <div class="prose prose-invert">
              <p class="text-content-400">
                Lorem ipsum, dolor sit amet consectetur adipisicing elit. Iusto, impedit.
              </p>
            </div>
          </div>
        </footer>
      </div>
    </template>
  </Modal>
</template>
