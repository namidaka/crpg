<script setup lang="ts">
import { FontAwesomeLayersText } from '@fortawesome/vue-fontawesome';
import { type CharacterArmorOverall } from '@/models/character';
import { ItemSlot } from '@/models/item';
import { type UserItem } from '@/models/user';

const {
  slot,
  placeholder,
  userItem,
  armorOverall,
  notMeetRequirement = false,
  available = false,
  focused = false,
  invalid = false,
  remove = false,
} = defineProps<{
  slot: ItemSlot;
  placeholder: string;
  userItem?: UserItem;
  armorOverall?: CharacterArmorOverall;
  notMeetRequirement: boolean;
  // slot state
  available?: boolean;
  focused?: boolean;
  invalid?: boolean;
  remove?: boolean;
}>();
</script>

<template>
  <div
    class="group relative flex h-28 items-center justify-center rounded-lg bg-base-200 ring"
    :class="[
      [available ? 'ring-border-300' : 'ring-transparent hover:ring-border-200'],
      {
        '!ring-status-success': focused,
        '!ring-status-warning': invalid,
        '!ring-status-danger': remove,
      },
    ]"
  >
    <ItemCard
      v-if="userItem !== undefined"
      :item="userItem.item"
      class="h-full cursor-grab !ring-0"
      :class="{ 'bg-primary-hover/15': userItem.isPersonal }"
      data-aq-character-slot-item-thumb
    >
      <template #badges-top-right>
        <Tag
          v-if="notMeetRequirement"
          rounded
          variant="danger"
          icon="alert"
          v-tooltip="$t('character.inventory.item.requirement.tooltip.title')"
        />
      </template>
    </ItemCard>

    <Tooltip
      v-else
      placement="bottom"
      :title="$t(`character.doll.slot.${slot}.title`)"
      :description="
        $t(`character.doll.slot.${slot}.description`) !== ''
          ? $t(`character.doll.slot.${slot}.description`)
          : undefined
      "
    >
      <OIcon
        class="select-none"
        :icon="placeholder"
        size="5x"
        data-aq-character-slot-item-placeholder
      />
    </Tooltip>

    <div
      v-if="armorOverall !== undefined"
      class="group absolute right-0 top-0 flex -translate-y-3/4 translate-x-1/2 transform cursor-default"
      v-tooltip.bottom="$t(`character.doll.armorOverall.${armorOverall.key}`)"
    >
      <FontAwesomeLayers class="fa-4x">
        <FontAwesomeIcon
          :icon="['crpg', 'fa-shield-duotone']"
          size="4x"
          class="text-base-400 group-hover:text-base-500"
          :style="{
            '--fa-secondary-opacity': 0.85,
          }"
        />
        <FontAwesomeLayersText
          :value="armorOverall.value"
          class="text-xs font-bold text-content-200 group-hover:text-content-100"
        />
      </FontAwesomeLayers>
    </div>
  </div>
</template>
