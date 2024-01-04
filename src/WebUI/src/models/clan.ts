import { Region } from '@/models/region';
import { UserItem, type UserPublic } from '@/models/user';

export interface Clan {
  id: number;
  tag: string;
  primaryColor: string;
  secondaryColor: string;
  name: string;
  bannerKey: string;
  region: Region;
  languages: ClanLanguage[];
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

export enum ClanLanguage {
  En = 'En', // English
  Zh = 'Zh', // Chinese
  Ru = 'Ru', // Russian
  De = 'De', // German
  Fr = 'Fr', // French
  It = 'It', // Italian
  Es = 'Es', // Spanish
  Pl = 'Pl', // Polish
  Uk = 'Uk', // Ukrainian
  Ro = 'Ro', // Romanian
  Nl = 'Nl', // Dutch
  Tr = 'Tr', // Turkish
  El = 'El', // Greek
  Hu = 'Hu', // Hungarian
  Sv = 'Sv', // Swedish
  Cs = 'Cs', // Czech
  Pt = 'Pt', // Portuguese
  Sr = 'Sr', // Serbian
  Bg = 'Bg', // Bulgarian
  Hr = 'Hr', // Croatian
  Da = 'Da', // Danish
  Fi = 'Fi', // Finnish
  No = 'No', // Norwegian
  Be = 'Be', // Belarusian
  Lv = 'Lv', // Latvian
}
