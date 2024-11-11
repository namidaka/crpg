import type { Clan } from './clan'
import type { Item, ItemSlot, ItemType } from './item'
import type { Platform } from './platform'
import type { Region } from './region'
import type Role from './role'

export interface User {
  id: number
  role: Role
  name: string
  gold: number
  avatar: string
  region: Region
  isDonor: boolean
  platform: Platform
  platformUserId: string
  heirloomPoints: number
  experienceMultiplier: number
  activeCharacterId: number | null
}

export interface UserPublic
  extends Pick<User, 'id' | 'platform' | 'platformUserId' | 'name' | 'region'> {
  avatar: string
  clan: Clan | null
}

export interface UserPrivate extends UserPublic {
  gold: number
  note: string
  createdAt: Date
  updatedAt: Date
  heirloomPoints: number
  experienceMultiplier: number
  activeCharacterId: number | null
}

// TODO: to /models/item.ts
export interface UserItem {
  id: number
  item: Item
  userId: number
  createdAt: Date
  isBroken: boolean
  isPersonal: boolean
  isArmoryItem: boolean
}

export interface UserItemsByType {
  type: ItemType
  items: UserItem[]
}

export type UserItemsBySlot = Record<ItemSlot, UserItem>
