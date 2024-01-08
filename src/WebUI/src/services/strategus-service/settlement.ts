import { SettlementType, type SettlementItem } from '@/models/strategus/settlement';
import { get, tryGet, post, put } from '@/services/crpg-client';

export const settlementIconByType: Record<
  SettlementType,
  {
    icon: string;
    iconSize: string;
  }
> = {
  [SettlementType.Town]: {
    icon: 'town',
    iconSize: 'lg',
  },
  [SettlementType.Castle]: {
    icon: 'castle',
    iconSize: 'sm',
  },
  [SettlementType.Village]: {
    icon: 'village',
    iconSize: 'sm',
  },
};

//
//
//
//
//
//
//
// TODO: Spec
export const getSettlementGarrisonItems = (id: number) =>
  get<SettlementItem[]>(`/settlements/${id}/items`);

export interface SettlementGarrisonItemsUpdate {
  itemId: string;
  count: number;
}

export const updateSettlementGarrisonItems = (id: number, payload: SettlementGarrisonItemsUpdate) =>
  post<SettlementItem>(`/settlements/${id}/items`, payload);
