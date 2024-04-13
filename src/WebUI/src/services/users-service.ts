import qs from 'qs';
import { type Item } from '@/models/item';
import {
  type User,
  type UserPublic,
  type UserItem,
  type UserItemsByType,
  type UserPrivate,
} from '@/models/user';
import { Platform } from '@/models/platform';
import { type Clan, type ClanEdition, type ClanMemberRole } from '@/models/clan';
import { type RestrictionWithActive, type PublicRestriction } from '@/models/restriction';
import { get, post, put, del } from '@/services/crpg-client';
import { mapRestrictions } from '@/services/restriction-service';
import { mapClanResponse } from '@/services/clan-service';
import { pick } from '@/utils/object';

export const getUser = () => get<User>('/users/self');

export const deleteUser = () => del('/users/self');

// TODO: SPEC
export const getUsersByIds = (payload: number[]) =>
  get<UserPrivate[]>(
    `/users?${qs.stringify(
      { id: payload },
      {
        arrayFormat: 'brackets',
      }
    )}`
  );

export const getUserById = (id: number) => get<UserPrivate>(`/users/${id}`);

export const updateUserNote = (id: number, user: { note: string }) =>
  put<UserPrivate>(`/users/${id}/note`, user);

interface UserSearchQuery {
  platform?: Platform;
  platformUserId?: string;
  name?: string;
}

export const searchUser = (payload: UserSearchQuery) =>
  get<UserPublic[]>(`/users/search/?${qs.stringify(payload)}`);

export const extractItemFromUserItem = (items: UserItem[]): Item[] => items.map(ui => ui.item);

export const getUserItems = () => get<UserItem[]>('/users/self/items');

export const buyUserItem = (itemId: string) => post<UserItem>('/users/self/items', { itemId });

export const repairUserItem = (userItemId: number) =>
  put<UserItem>(`/users/self/items/${userItemId}/repair`);

export const upgradeUserItem = (userItemId: number) =>
  put<UserItem>(`/users/self/items/${userItemId}/upgrade`);

export const reforgeUserItem = (userItemId: number) =>
  put<UserItem>(`/users/self/items/${userItemId}/reforge`);

export const rewardUser = (userId: number, payload: { gold: number; heirloomPoints: number }) =>
  put(`/users/${userId}/rewards`, payload);

export const sellUserItem = (userItemId: number) => del(`/users/self/items/${userItemId}`);

export const groupUserItemsByType = (items: UserItem[]) =>
  items
    .reduce((itemsGroup, ui) => {
      const type = ui.item.type;
      const currentGroup = itemsGroup.find(item => item.type === type);

      if (currentGroup) {
        currentGroup.items.push(ui);
      } else {
        itemsGroup.push({
          type,
          items: [ui],
        });
      }

      return itemsGroup;
    }, [] as UserItemsByType[])
    .sort((a, b) => a.type.localeCompare(b.type));

interface UserClan {
  clan: ClanEdition;
  role: ClanMemberRole;
}

export const getUserClan = async () => {
  const userClan = await get<UserClan | null>('/users/self/clan');
  if (userClan === null || userClan.clan === null) return null;
  // do conversion since argb values are stored as numbers in db and we need strings
  return { clan: mapClanResponse(userClan.clan), role: userClan.role };
};

export const getUserRestriction = () => get<PublicRestriction>('/users/self/restriction');

export const getUserRestrictions = async (id: number) =>
  mapRestrictions(await get<RestrictionWithActive[]>(`/users/${id}/restrictions`));

export const mapUserToUserPublic = (user: User, userClan: Clan | null): UserPublic => ({
  ...pick(user, ['id', 'platform', 'platformUserId', 'name', 'region', 'avatar']),
  clan: userClan,
});
