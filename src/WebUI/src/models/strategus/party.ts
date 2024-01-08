import { type MultiPoint, type Point } from 'geojson';
import { type UserPublic, type UserClan } from '@/models/user';
import { type SettlementPublic } from '@/models/strategus/settlement';

export enum PartyStatus {
  Idle = 'Idle',
  IdleInSettlement = 'IdleInSettlement',
  RecruitingInSettlement = 'RecruitingInSettlement',
  MovingToPoint = 'MovingToPoint',
  FollowingParty = 'FollowingParty',
  MovingToSettlement = 'MovingToSettlement',
  MovingToAttackParty = 'MovingToAttackParty',
  MovingToAttackSettlement = 'MovingToAttackSettlement',
  InBattle = 'InBattle',
}

export interface PartyCommon {
  id: number;
  name: string;
  troops: number;
  position: Point;
  user: UserPublic;
  clan: UserClan | null;
}

export interface Party extends PartyCommon {
  gold: number;
  status: PartyStatus;
  waypoints: MultiPoint;
  targetedParty: PartyCommon | null;
  targetedSettlement: SettlementPublic | null;
}

export interface PartyStatusUpdateRequest {
  status: PartyStatus;
  waypoints: MultiPoint;
  targetedPartyId: number;
  targetedSettlementId: number;
}
