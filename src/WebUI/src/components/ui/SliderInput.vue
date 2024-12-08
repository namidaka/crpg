<script setup lang="ts">
import { clamp } from 'es-toolkit'

const props = defineProps<{
  min: number
  max: number
  step: number
}>()

const modelValue = defineModel<number[]>({ default: () => [] })

// TODO: SPEC
const localValue = computed({
  get() {
    const [from, to] = modelValue.value

    if (!from && !to) {
      // console.log('get [from, to]', [from, to]);
      // console.log('get [props.min, props.max]', [props.min, props.max]);

      // in [] out [min, max]
      return [props.min, props.max]
    }
    else if ((to === undefined || to === null) && (from !== undefined || from !== null)) {
      // console.log('get 2', [from, to]);

      // in [val, null] out [val, max]
      return [Math.max(from, props.min), props.max]
    }
    else if ((from === undefined || from === null) && (to !== undefined || to !== null)) {
      // console.log('get 3', [from, to]);

      // in [null, val] out [min, val]
      return [props.min, Math.min(to, props.max)]
    }
    else {
      // console.log('get 4', [from, to], [Math.max(from, props.min), Math.min(to, props.max)]);

      // out [val1, val2] out [val1, val2]
      return [Math.max(from, props.min), Math.min(to, props.max)]
    }
  },

  set(value) {
    const [_from, _to] = value

    let from = _from
    let to = _to

    // console.log('set [from, to]', [from, to]);
    // console.log('set [Math.max(from, props.min), Math.min(to, props.max)]', [
    //   Math.max(from, props.min),
    //   Math.min(to, props.max),
    // ]);

    if (from > to || to < from) {
      [from, to] = [to, from] // swap
    }

    from = clamp(from, props.min, props.max)
    to = clamp(to, props.min, props.max)

    // Empty with default values. Not to flood the query string ;)
    if (from === props.min && to === props.max) {
      modelValue.value = []
    }
    else {
      modelValue.value = [from, to]
    }

    nextTick().then(forceRerender) // TODO: Fix native-input display bug, ref: https://michaelnthiessen.com/force-re-render/
  },
})

const change = (from: number, to: number) => {
  localValue.value = [from, to]
}

const inputComponentKey = ref(0)

const forceRerender = () => {
  inputComponentKey.value += 1
}
</script>

<template>
  <div class="space-y-8 px-2 pt-7">
    <VueSlider
      v-model="localValue"
      lazy
      :min="min"
      :tooltip-formatter="$n"
      :max="max"
      :interval="step"
      :marks="[min, max]"
    >
      <template #mark="{ pos, value, label }">
        <div
          class="absolute top-2.5 whitespace-nowrap text-2xs text-content-400"
          :class="{
            '-translate-x-full': value === max,
          }"
          :style="{ left: `${pos}%` }"
        >
          {{ $n(label) }}
        </div>
      </template>
    </VueSlider>

    <div class="flex w-full justify-between gap-4">
      <OInput
        :key="inputComponentKey"
        class="w-24"
        :model-value="localValue[0]"
        size="sm"
        type="number"
        :min="min"
        :max="max"
        :icon-right="min !== localValue[0] ? 'close' : undefined"
        icon-right-clickable
        @icon-right-click="change(min, localValue[1])"
        @update:model-value="() => { }"
        @change="
          (event: Event) => change(Number((event.target as HTMLInputElement).value), localValue[1])
        "
      />
      <OInput
        :key="inputComponentKey"
        class="w-24"
        :model-value="localValue[1]"
        size="sm"
        type="number"
        :min="min"
        :max="max"
        :icon-right="max !== localValue[1] ? 'close' : undefined"
        icon-right-clickable
        @icon-right-click="change(localValue[0], max)"
        @update:model-value="() => { }"
        @change="
          (event: Event) => change(localValue[0], Number((event.target as HTMLInputElement).value))
        "
      />
    </div>
  </div>
</template>
