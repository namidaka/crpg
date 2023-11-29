<script setup lang="ts">
import { defaultGold } from '@root/data/constants.json';
import { characterClassToIcon } from '@/services/characters-service';
import { CharacterClass } from '@/models/character';

defineEmits<{
  startOnboarding: [];
}>();

const presets = ref([
  {
    id: 1,
    gold: 30000,
    class: CharacterClass.Infantry,
    level: 30,
    description: '',
  },
  {
    id: 2,
    gold: 200000,
    class: CharacterClass.Peasant,
    level: 0,
    description: '',
  },
]);

const presetModel = ref(1);
</script>

<template>
  <Modal closable shown>
    <template #popper>
      <div class="flex max-h-[90vh] w-[40rem] flex-col">
        <header class="relative h-[11rem]">
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

          <div class="prose prose-invert">
            <p class="text-center">Pick your start and go kick some ass.</p>

            <div class="flex justify-center">
              <VDropdown :triggers="['click']" placement="bottom-end" class="">
                <template #default="{ shown }">
                  <OButton variant="primary" outlined size="lg">
                    you get
                    <Coin :value="defaultGold" v-tooltip.bottom="$t('user.field.gold')" />
                    and
                    <OIcon :icon="characterClassToIcon[CharacterClass.Infantry]" size="lg" />
                    {{ $t(`character.class.${CharacterClass.Infantry}`) }}
                    <div
                      class="flex items-center gap-2 font-bold"
                      v-tooltip.bottom="'Character level'"
                    >
                      30
                    </div>

                    <!-- TODO: to cmp -->
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
                  <DropdownItem class="text-primary" v-for="preset in presets">
                    <Coin :value="preset.gold" v-tooltip.bottom="$t('user.field.gold')" />

                    <OIcon :icon="characterClassToIcon[preset.class]" size="lg" />
                    {{ $t(`character.class.${preset.class}`) }}
                    <div
                      class="flex items-center gap-2 font-bold"
                      v-tooltip.bottom="'Character level'"
                    >
                      {{ preset.level }}
                    </div>
                  </DropdownItem>
                </template>
              </VDropdown>
            </div>
          </div>

          <Divider />

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

          <div class="space-y-6">
            <FormGroup icon="help-circle" label="Helpful links" collapsed>
              <!--  -->
              <div>TODO:</div>
            </FormGroup>

            <FormGroup icon="settings" label="Some interesting" collapsed>
              <!--  -->
              <div>TODO:</div>
            </FormGroup>
          </div>
        </div>

        <footer>
          <Divider />

          <div class="prose prose-invert px-12 py-6">
            <p class="text-content-400">
              Lorem ipsum, dolor sit amet consectetur adipisicing elit. Iusto, impedit.
            </p>
          </div>
        </footer>
      </div>
    </template>
  </Modal>
</template>
