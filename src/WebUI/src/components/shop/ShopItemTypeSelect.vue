<script setup lang="ts">
import type { ItemType, WeaponClass } from '~/models/item'
import type { Buckets } from '~/models/item-search'

import {
  hasWeaponClassesByItemType,
  humanizeBucket, // TODO: FIXME:
  itemTypeByWeaponClass,
} from '~/services/item-service'

const props = defineProps<{
  itemType: ItemType
  weaponClass: WeaponClass | null
  itemTypeBuckets: Buckets
  weaponClassBuckets: Buckets
}>()

const emit = defineEmits<{
  (e: 'update:itemType', val: ItemType): void
  (e: 'update:weaponClass', val: WeaponClass | null): void
}>()

const itemTypeModel = computed({
  get() {
    return props.itemType
  },

  set(val: ItemType) {
    emit('update:itemType', val)
  },
})

const weaponClassModel = computed({
  get() {
    return props.weaponClass
  },

  set(val: WeaponClass | null) {
    emit('update:weaponClass', val)
  },
})

const subLevelActive = computed(
  () => hasWeaponClassesByItemType(itemTypeModel.value) && weaponClassModel.value !== null,
)
</script>

<template>
  <OTabs
    v-model="itemTypeModel"
    content-class="hidden"
    :type="subLevelActive ? 'fill-rounded-grouped' : 'fill-rounded'"
    :animated="false"
  >
    <OTabItem
      v-for="ItemTypebucket in itemTypeBuckets"
      :key="(ItemTypebucket.key as string)"
      :value="(ItemTypebucket.key as string)"
      tag="div"
    >
      <template #header>
        <OIcon
          v-tooltip.bottom="humanizeBucket('type', ItemTypebucket.key)!.label"
          :icon="humanizeBucket('type', ItemTypebucket.key)!.icon!.name"
          size="2xl"
        />

        <template
          v-if="
            subLevelActive
              && weaponClassModel !== null
              && (ItemTypebucket.key as ItemType) === itemTypeByWeaponClass[weaponClassModel]
          "
        >
          <OIcon
            icon="chevron-right"
            size="lg"
            class="text-content-400"
          />

          <OTabs
            v-model="weaponClassModel"
            type="flat-rounded"
            content-class="hidden"
            :animated="false"
          >
            <OTabItem
              v-for="weaponClassBucket in weaponClassBuckets"
              :key="(weaponClassBucket.key as string)"
              :value="(weaponClassBucket.key as string)"
            >
              <template #header>
                <OIcon
                  v-tooltip.bottom="humanizeBucket('weaponClass', weaponClassBucket.key)!.label"
                  :icon="humanizeBucket('weaponClass', weaponClassBucket.key)!.icon!.name"
                  size="2xl"
                />
              </template>
            </OTabItem>
          </OTabs>
        </template>
      </template>
    </OTabItem>
  </OTabs>
</template>
