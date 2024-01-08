import { type Point } from 'geojson';
import { type Item } from '@/models/item';
import { type UserPublic } from '@/models/user';
import { Culture } from '@/models/culture';

export enum SettlementType {
  Village = 'Village',
  Castle = 'Castle',
  Town = 'Town',
}

export interface SettlementPublic {
  id: number;
  name: string;
  type: SettlementType;
  culture: Culture;
  position: Point;
  scene: string;
  region: string;
  troops: number;
  owner: UserPublic | null;
}

export interface SettlementItem {
  item: Item;
  count: number;
}
