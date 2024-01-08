import {
  SettlementType,
  type SettlementItem,
  type SettlementPublic,
} from '@/models/strategus/settlement';
import { type UserPublic } from '@/models/user';
import { type Clan } from '@/models/clan';

import { get, tryGet, post, put } from '@/services/crpg-client';
import { argbIntToRgbHexColor } from '@/utils/color';

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

const mapOwnerClan = (clan: Clan | null) => {
  return clan === null
    ? null
    : {
        ...clan,
        primaryColor: argbIntToRgbHexColor(Number(clan.primaryColor)),
        secondaryColor: argbIntToRgbHexColor(Number(clan.secondaryColor)),
      };
};

const mapSettlementOwner = (owner: UserPublic | null) => {
  return owner === null
    ? null
    : {
        ...owner,
        clan: mapOwnerClan(owner.clan),
      };
};

export const getSettlements = async () => {
  const res = await get<SettlementPublic[]>('/settlements');

  return res.map(sp => {
    return {
      ...sp,
      owner: mapSettlementOwner(sp.owner),
    };
  });
};

export const getSettlement = (id: number) => get<SettlementPublic>(`/settlements/${id}`);

export const getSettlementGarrisonItems = (id: number) =>
  get<SettlementItem[]>(`/settlements/${id}/items`);

export interface SettlementGarrisonItemsUpdate {
  itemId: string;
  count: number;
}

export const updateSettlementGarrisonItems = (id: number, payload: SettlementGarrisonItemsUpdate) =>
  post<SettlementItem>(`/settlements/${id}/items`, payload);
