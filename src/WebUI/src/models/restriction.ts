import type { UserPrivate, UserPublic } from '~/models/user'

export enum RestrictionType {
  Join = 'Join',
  Chat = 'Chat',
  All = 'All',
}

export interface Restriction {
  id: number
  reason: string
  createdAt: Date
  duration: number
  publicReason: string
  type: RestrictionType
  restrictedUser: UserPrivate
  restrictedByUser: UserPublic
}

export interface PublicRestriction {
  id: number
  reason: string
  createdAt: Date
  duration: number
}

export interface RestrictionWithActive extends Restriction {
  active: boolean
}

export interface RestrictionCreation {
  reason: string
  duration: number
  publicReason: string
  type: RestrictionType
  restrictedUserId: number
}
