<script setup lang="ts">
import { DateTime, type DurationLike } from 'luxon';
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
import { type TimeSeries } from '@/models/timeseries';
import theme from '@/assets/themes/oruga-tailwind/echart-theme.json';
import { d } from '@/services/translate-service';
import { getCharacterStatisticsCharts } from '@/services/characters-service';
import { characterKey } from '@/symbols/character';
import { sleep } from '@/utils/promise';

use([ToolboxComponent, BarChart, TooltipComponent, LegendComponent, GridComponent, SVGRenderer]);
registerTheme('crpg', theme);
type EChartsOption = ComposeOption<
  | LegendComponentOption
  | ToolboxComponentOption
  | TooltipComponentOption
  | GridComponentOption
  | BarSeriesOption
>;
// TODO:
enum Zoom {
  '1h' = '1h',
  '3h' = '3h',
  '12h' = '12h',
  '2d' = '2d',
  '7d' = '7d',
  '14d' = '14d',
}

// TODO:
enum StatType {
  'Exp' = 'Exp',
  'Gold' = 'Gold',
}

interface LegendSelectEvent {
  name: string;
  type: 'legendselectchanged';
  selected: Record<string, boolean>;
}

const character = injectStrict(characterKey);

const loading = ref(false);
const loadingOptions = {
  text: 'Loading…',
  color: '#4ea397',
  maskColor: 'rgba(255, 255, 255, 0.4)',
};

const durationByZoom: Record<Zoom, DurationLike> = {
  [Zoom['1h']]: {
    hours: 1,
  },
  [Zoom['3h']]: {
    hours: 3,
  },
  [Zoom['12h']]: {
    hours: 12,
  },
  [Zoom['2d']]: {
    days: 2,
  },
  [Zoom['7d']]: {
    days: 7,
  },
  [Zoom['14d']]: {
    days: 14,
  },
};

const getStart = (zoom: Zoom) => {
  switch (zoom) {
    case Zoom['1h']:
    default:
      return DateTime.local().minus(durationByZoom[Zoom['1h']]).toJSDate();
    case Zoom['3h']:
      return DateTime.local().minus(durationByZoom[Zoom['3h']]).toJSDate();
    case Zoom['12h']:
      return DateTime.local().minus(durationByZoom[Zoom['12h']]).toJSDate();
    case Zoom['2d']:
      return DateTime.local().minus(durationByZoom[Zoom['2d']]).toJSDate();
    case Zoom['7d']:
      return DateTime.local().minus(durationByZoom[Zoom['7d']]).toJSDate();
    case Zoom['14d']:
      return DateTime.local().minus(durationByZoom[Zoom['14d']]).toJSDate();
  }
};

const zoomModel = ref<Zoom>(Zoom['1h']);
const start = computed(() => getStart(zoomModel.value));

const statTypeModel = ref<StatType>(StatType['Exp']);
const { state: characterStatistics, execute: loadCharacterStatistics } = await useAsyncState<
  TimeSeries[]
>(() => getCharacterStatisticsCharts(character.value.id, statTypeModel.value, start.value), [], {
  resetOnExecute: false,
});

const legend = ref<string[]>(characterStatistics.value.map(ts => ts.name));
const activeSeries = ref<string[]>(characterStatistics.value.map(ts => ts.name));

const onUpdate = async () => {
  await loadCharacterStatistics();
  option.value = {
    ...option.value,
    series: characterStatistics.value.map(ts => ({ ...ts, type: 'bar' })),
    legend: {
      ...option.value.legend,
      data: characterStatistics.value.map(ts => ts.name),
    },
  };
  activeSeries.value = characterStatistics.value.map(ts => ts.name);
};
watch(statTypeModel, async () => {
  await onUpdate();
});
watch(zoomModel, async () => {
  setZoom();
  await onUpdate();
});

const total = computed(() =>
  characterStatistics.value
    .filter(ts => activeSeries.value.includes(ts.name))
    .flatMap(ts => ts.data)
    .reduce((total, [_date, value]) => total + value, 0)
);

const chart = shallowRef<InstanceType<typeof VChart> | null>(null);

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
  end.value = new Date();
  option.value = {
    ...option.value,
    xAxis: {
      ...option.value.xAxis,
      min: start.value,
      max: end.value,
    },
  };
};

const onLegendSelectChanged = (e: LegendSelectEvent) => {
  activeSeries.value = Object.entries(e.selected)
    .filter(([_legend, status]) => Boolean(status))
    .map(([legend, _status]) => legend);
};
</script>

<template>
  <div class="flex max-h-[90vh] min-w-[48rem] flex-col pl-5 pr-10 pt-8">
    <div class="flex items-center gap-4">
      <OTabs v-model="statTypeModel" type="fill-rounded" contentClass="hidden">
        <OTabItem :value="StatType['Exp']" :label="`Exp`" />
        <OTabItem :value="StatType['Gold']" :label="`Gold`" />
      </OTabs>
      <OTabs v-model="zoomModel" type="fill-rounded" contentClass="hidden">
        <OTabItem
          v-for="(zoomValue, zoomKey) in durationByZoom"
          :value="zoomKey"
          :label="
            $t(
              `dateTimeFormat.${Object.keys(zoomValue).includes('days') ? 'dd' : 'hh'}`,
              zoomValue as any
            )
          "
        />
      </OTabs>
      <div class="flex-1 text-lg font-semibold">
        <Coin
          v-if="statTypeModel === StatType.Gold"
          :value="total"
          :class="total < 0 ? 'text-status-danger' : 'text-status-success'"
        />
        <div v-else class="flex items-center gap-1.5 align-text-bottom font-bold text-primary">
          <OIcon icon="experience" size="2xl" />
          <span class="leading-none">{{ $n(total) }}</span>
        </div>
      </div>
    </div>

    <VChart
      class="h-[30rem]"
      ref="chart"
      theme="crpg"
      :option="option"
      :loading="loading"
      :loadingOptions="loadingOptions"
      @legendselectchanged="onLegendSelectChanged"
    />
  </div>
</template>
