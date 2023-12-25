<script setup lang="ts">
import { use, registerTheme, type ComposeOption } from 'echarts/core';
import { BarChart, type BarSeriesOption } from 'echarts/charts';
import {
  ToolboxComponent,
  type ToolboxComponentOption,
  GridComponent,
  type GridComponentOption,
  TooltipComponent,
  type TooltipComponentOption,
  LegendComponent,
  type LegendComponentOption,
} from 'echarts/components';
import { SVGRenderer } from 'echarts/renderers';
import VChart from 'vue-echarts';
import theme from '@/assets/themes/oruga-tailwind/echart-theme.json';
import { d } from '@/services/translate-service';
import { getCharacterStatisticsCharts } from '@/services/characters-service';
import { characterKey } from '@/symbols/character';

use([ToolboxComponent, BarChart, TooltipComponent, LegendComponent, GridComponent, SVGRenderer]);
registerTheme('crpg', theme);
type EChartsOption = ComposeOption<
  | LegendComponentOption
  | ToolboxComponentOption
  | TooltipComponentOption
  | GridComponentOption
  | BarSeriesOption
>;
import { DateTime } from 'luxon';

definePage({
  props: true,
  meta: {
    layout: 'default',
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const character = injectStrict(characterKey);

const loading = ref(false);
const loadingOptions = {
  text: 'Loading…',
  color: '#4ea397',
  maskColor: 'rgba(255, 255, 255, 0.4)',
};

interface TimeSeries {
  name: string;
  data: [Date, number][];
}

// TODO:
enum StatType {
  'Exp' = 'Exp',
  'Gold' = 'Gold',
}

const statTypeModel = ref<StatType>(StatType['Exp']);
const { state: characterStatistics, execute: loadCharacterStatistics } = await useAsyncState<
  TimeSeries[]
>(() => getCharacterStatisticsCharts(character.value.id, statTypeModel.value), [], {
  immediate: true,
  resetOnExecute: false,
});
watch(statTypeModel, async () => {
  await loadCharacterStatistics();
  option.value = {
    ...option.value,
    series: characterStatistics.value.map(ts => ({ ...ts, type: 'bar' })),
  };
});

const legend = ref<string[]>(characterStatistics.value.map(ts => ts.name));
const activeSeries = ref<string[]>(characterStatistics.value.map(ts => ts.name));

const total = computed(() =>
  characterStatistics.value
    .filter(ts => activeSeries.value.includes(ts.name))
    .flatMap(ts => ts.data)
    .filter(([date]) => date > start.value && date < end.value)
    .reduce((total, [_date, value]) => total + value, 0)
);

enum Zoom {
  '1h' = '1h',
  '3h' = '3h',
  '12h' = '12h',
  '2d' = '2d',
  '7d' = '7d',
  '14d' = '14d',
}

const chart = shallowRef<InstanceType<typeof VChart> | null>(null);
const zoomModel = ref<Zoom>(Zoom['1h']);

const getStart = (zoom: Zoom) => {
  switch (zoom) {
    default:
    case Zoom['1h']:
      return DateTime.local().minus({ hours: 1 }).toJSDate();
    case Zoom['3h']:
      return DateTime.local().minus({ hours: 3 }).toJSDate();
    case Zoom['12h']:
      return DateTime.local().minus({ hours: 12 }).toJSDate();
    case Zoom['2d']:
      return DateTime.local().minus({ days: 2 }).toJSDate();
    case Zoom['7d']:
      return DateTime.local().minus({ days: 7 }).toJSDate();
    case Zoom['14d']:
      return DateTime.local().minus({ days: 14 }).toJSDate();
  }
};

const start = computed(() => getStart(zoomModel.value));
const end = ref<Date>(new Date());

const option = shallowRef<EChartsOption>({
  xAxis: {
    type: 'time',
    min: getStart(Zoom['1h']),
    max: Date.now(),
    splitLine: {
      show: false,
    },
    splitArea: {
      show: false,
    },
  },
  yAxis: {
    type: 'value',
    splitArea: {
      show: false,
    },
  },
  legend: {
    data: legend.value,
    orient: 'vertical',
    top: 'center',
    itemGap: 16,
    right: 0,
  },
  toolbox: {
    show: false, // TODO:
    feature: {
      dataView: { show: true, readOnly: false },
      saveAsImage: { show: true },
    },
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: {
      type: 'shadow',
      label: {
        formatter: param => d(new Date(param.value), 'long'),
      },
    },
  },
  series: characterStatistics.value.map(ts => ({ ...ts, type: 'bar' })),
});

const setZoom = () => {
  end.value = new Date().getTime();
  option.value = {
    ...option.value,
    xAxis: {
      ...option.value.xAxis,
      min: start.value,
      max: end.value,
    },
  };
};

interface LegendSelectEvent {
  name: string;
  type: 'legendselectchanged';
  selected: Record<string, boolean>;
}

const onLegendSelectChanged = (e: LegendSelectEvent) => {
  activeSeries.value = Object.entries(e.selected)
    .filter(([_legend, status]) => Boolean(status))
    .map(([legend, _status]) => legend);
};

watch(
  zoomModel,
  () => {
    setZoom();
  },
  { immediate: true }
);
</script>

<template>
  <div>
    <div class="mx-auto">
      <div class="flex items-center justify-center gap-8">
        <OTabs v-model="statTypeModel" type="fill-rounded" contentClass="hidden">
          <OTabItem :value="StatType['Exp']" :label="`Exp.`" />
          <OTabItem :value="StatType['Gold']" :label="`Gold`" />
        </OTabs>
        <OTabs v-model="zoomModel" type="fill-rounded" contentClass="hidden">
          <OTabItem :value="Zoom['1h']" :label="$t('dateTimeFormat.hh', { hours: 1 })" />
          <OTabItem :value="Zoom['3h']" :label="$t('dateTimeFormat.hh', { hours: 3 })" />
          <OTabItem :value="Zoom['12h']" :label="$t('dateTimeFormat.hh', { hours: 12 })" />
          <OTabItem :value="Zoom['2d']" :label="$t('dateTimeFormat.dd', { days: 2 })" />
          <OTabItem :value="Zoom['7d']" :label="$t('dateTimeFormat.dd', { days: 7 })" />
          <OTabItem :value="Zoom['14d']" :label="$t('dateTimeFormat.dd', { days: 14 })" />
        </OTabs>
        <div>
          <div class="text-lg font-semibold text-primary">
            {{ $n(total) }} {{ statTypeModel === StatType.Exp ? 'exp.' : 'gold' }}
          </div>
        </div>
      </div>
      <VChart
        class="h-[40rem]"
        ref="chart"
        theme="crpg"
        :option="option"
        autoresize
        :loading="loading"
        :loadingOptions="loadingOptions"
        @legendselectchanged="onLegendSelectChanged"
      />
    </div>
  </div>
</template>
