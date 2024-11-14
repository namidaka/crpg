import type {
  Restriction,
  RestrictionCreation,
  RestrictionWithActive,
} from '~/models/restriction'

import { get, post } from '~/services/crpg-client'
import { checkIsDateExpired } from '~/utils/date'

export const checkIsRestrictionActive = (
  restrictions: Restriction[],
  { createdAt, id, restrictedUser, type }: Restriction,
): boolean => {
  return !restrictions.some(
    r =>
      restrictedUser!.id === r.restrictedUser!.id // groupBy user - there may be restrisctions for other users on the list (/admin page)
      && type === r.type
      && id !== r.id
      && createdAt.getTime() < r.createdAt.getTime(), // check whether the the current restriction is NOT the newest
  )
}

export const mapRestrictions = (restrictions: Restriction[]): RestrictionWithActive[] => {
  return restrictions.map((r) => {
    const isExpired = checkIsDateExpired(r.createdAt, r.duration)
    const isRestrictionActive = checkIsRestrictionActive(restrictions, r)

    return ({
      ...r,
      active:
          !isExpired && isRestrictionActive,
    })
  })
}

export const getRestrictions = async () =>
  mapRestrictions(await get<Restriction[]>('/restrictions'))

export const restrictUser = (payload: RestrictionCreation) =>
  post<Restriction>('/restrictions', payload)
