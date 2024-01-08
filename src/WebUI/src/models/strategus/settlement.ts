import { type Point } from 'geojson';
import { type Item } from '../item';
import { Party } from './party';

export enum SettlementType {
  Village = 'Village',
  Castle = 'Castle',
  Town = 'Town',
}

export interface SettlementPublic {
  id: number;
  name: string;
  type: SettlementType;
  culture: string;
  position: Point;
  scene: string;
  region: string;
  troops: number;
  owner: Party | null;
}

export interface SettlementItem {
  item: Item;
  count: number;
}
