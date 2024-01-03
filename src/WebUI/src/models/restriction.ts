import { type UserPublic, type UserPrivate } from '@/models/user';

export enum RestrictionType {
  Join = 'Join',
  Chat = 'Chat',
  All = 'All',
}

export interface Restriction {
  id: number;
  restrictedUser: UserPrivate;
  duration: number;
  type: RestrictionType;
  reason: string;
  publicReason: string;
  restrictedByUser: UserPublic;
  createdAt: Date;
}

export interface PublicRestriction {
  id: number;
  createdAt: Date;
  duration: number;
  reason: string;
}

export interface RestrictionWithActive extends Restriction {
  active: boolean;
}

export interface RestrictionCreation {
  restrictedUserId: number;
  type: RestrictionType;
  reason: string;
  publicReason: string;
  duration: number;
}
