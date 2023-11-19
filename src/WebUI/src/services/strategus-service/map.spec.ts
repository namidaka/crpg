import { LatLngBounds } from 'leaflet';
import { shouldDisplaySettlement } from './map';
import { SettlementType, type SettlementPublic } from '@/models/strategus/settlement';

it.each<[SettlementPublic, LatLngBounds, number, boolean]>([
  [
    {
      position: {
        type: 'Point',
        coordinates: [0.5, 0.5],
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
        type: 'Point',
        coordinates: [0.5, 0.5],
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
        type: 'Point',
        coordinates: [0.5, 0.5],
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
        type: 'Point',
        coordinates: [0.5, 0.5],
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
        type: 'Point',
        coordinates: [0.5, 0.5],
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
  expect(shouldDisplaySettlement(settlement, bounds, zoom)).toEqual(expectation);
});
