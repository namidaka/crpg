import { Platform } from '@/models/platform';
import Role from '@/models/role';
import { Region } from '@/models/region';
import { ItemSlot, ItemType, type Item } from '@/models/item';
import { type Clan, ClanMemberRole } from '@/models/clan';

export interface User {
  id: number;
  platform: Platform;
  platformUserId: string;
  name: string;
  gold: number;
  heirloomPoints: number;
  role: Role;
  avatar: string;
  activeCharacterId: number | null;
  region: Region;
  experienceMultiplier: number;
  isDonor: boolean;
}

export interface UserPublic
  extends Pick<User, 'id' | 'platform' | 'platformUserId' | 'name' | 'region'> {
  avatar: string;
  clan: UserClan | null;
}

export interface UserPrivate extends UserPublic {
  createdAt: Date;
  updatedAt: Date;
  gold: number;
  heirloomPoints: number;
  experienceMultiplier: number;
  note: string;
  activeCharacterId: number | null;
}

// TODO: to /models/item.ts
export interface UserItem {
  id: number;
  userId: number;
  createdAt: Date;
  item: Item;
  isBroken: boolean;
  isArmoryItem: boolean;
}

export interface UserItemsByType {
  type: ItemType;
  items: UserItem[];
}

export interface UserClan {
  clan: Clan;
  role: ClanMemberRole;
}

export type UserItemsBySlot = Record<ItemSlot, UserItem>;
