import { LatLngBounds } from 'leaflet'

import type { SettlementPublic } from '~/models/strategus/settlement'

import { SettlementType } from '~/models/strategus/settlement'

import { shouldDisplaySettlement } from './map'

it.each<[SettlementPublic, LatLngBounds, number, boolean]>([
  [
    {
      position: {
        coordinates: [0.5, 0.5],
        type: 'Point',
      },
      type: SettlementType.Village,
    } as SettlementPublic,
    new LatLngBounds([
      [0, 0],
      [1, 1],
    ]),
    3,
    false,
  ],
  [
    {
      position: {
        coordinates: [0.5, 0.5],
        type: 'Point',
      },
      type: SettlementType.Castle,
    } as SettlementPublic,
    new LatLngBounds([
      [0, 0],
      [1, 1],
    ]),
    3,
    false,
  ],
  [
    {
      position: {
        coordinates: [0.5, 0.5],
        type: 'Point',
      },
      type: SettlementType.Castle,
    } as SettlementPublic,
    new LatLngBounds([
      [0, 0],
      [1, 1],
    ]),
    4,
    true,
  ],
  [
    {
      position: {
        coordinates: [0.5, 0.5],
        type: 'Point',
      },
      type: SettlementType.Village,
    } as SettlementPublic,
    new LatLngBounds([
      [0, 0],
      [1, 1],
    ]),
    4,
    false,
  ],
  [
    {
      position: {
        coordinates: [0.5, 0.5],
        type: 'Point',
      },
      type: SettlementType.Town,
    } as SettlementPublic,
    new LatLngBounds([
      [0, 0],
      [1, 1],
    ]),
    3,
    true,
  ],
])('shouldDisplaySettlement - items: %j', (settlement, bounds, zoom, expectation) => {
  expect(shouldDisplaySettlement(settlement, bounds, zoom)).toEqual(expectation)
})
