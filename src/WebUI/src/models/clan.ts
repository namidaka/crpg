import { Region } from '@/models/region';
import { UserItem, type UserPublic } from '@/models/user';
import { Language } from '@/models/language';

export interface Clan {
  id: number;
  tag: string;
  primaryColor: string;
  secondaryColor: string;
  name: string;
  bannerKey: string;
  region: Region;
  languages: Language[];
  discord: string | null;
  description: string;
  armoryTimeout: number;
}

// TODO: rename
export interface ClanEdition extends Omit<Clan, 'primaryColor' | 'secondaryColor'> {
  primaryColor: number;
  secondaryColor: number;
}

export interface ClanWithMemberCount<T> {
  clan: T;
  memberCount: number;
}

export interface ClanMember {
  user: UserPublic;
  role: ClanMemberRole;
}

export enum ClanMemberRole {
  Member = 'Member',
  Officer = 'Officer',
  Leader = 'Leader',
}

export enum ClanInvitationType {
  Request = 'Request',
  Offer = 'Offer',
}

export enum ClanInvitationStatus {
  Pending = 'Pending',
  Declined = 'Declined',
  Accepted = 'Accepted',
}

export interface ClanInvitation {
  id: number;
  invitee: UserPublic;
  inviter: UserPublic;
  type: ClanInvitationType;
  status: ClanInvitationStatus;
}

export interface BorrowedItem {
  updatedAt: Date;
  borrowerUserId: number;
  userItemId: number;
}

export interface ClanArmoryItem {
  userItem: UserItem;
  borrowedItem: BorrowedItem | null;
  updatedAt: Date;
}
