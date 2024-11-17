import type { SettlementPublic } from '@/models/strategus/settlement';

export const settlementKey: InjectionKey<Ref<SettlementPublic | null>> = Symbol('Settlement');
