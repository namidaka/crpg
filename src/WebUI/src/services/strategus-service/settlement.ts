import { SettlementType } from '@/models/strategus/settlement';

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
