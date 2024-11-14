import type { ItemFieldCompareRule, ItemFieldFormat, ItemFlat } from '~/models/item'

export enum AggregationView {
  Range = 'Range',
  Checkbox = 'Checkbox',
  Radio = 'Radio',
}

export type FiltersModel<T> = Partial<Record<keyof ItemFlat, T>>

export interface Aggregation extends Omit<itemsjs.Aggregation, 'title'> {
  title?: string
  width?: number
  hidden?: boolean // don't display in the table
  description?: string
  view: AggregationView
  format?: ItemFieldFormat
  compareRule?: ItemFieldCompareRule // for compare mode
}

export type AggregationConfig = Partial<Record<keyof ItemFlat, Aggregation>>

export type SortingConfig = Record<string, itemsjs.Sorting<ItemFlat>>

export type Buckets = itemsjs.Buckets<ItemFlat[keyof ItemFlat]>
