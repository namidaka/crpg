import {
  type Clan,
  type ClanWithMemberCount,
  type ClanEdition,
  type ClanMember,
  ClanMemberRole,
  type ClanInvitation,
  ClanInvitationType,
  ClanInvitationStatus,
  type ClanArmoryItem,
} from '@/models/clan';
import { Region } from '@/models/region';
import { get, post, put, del } from '@/services/crpg-client';
import { rgbHexColorToArgbInt, argbIntToRgbHexColor } from '@/utils/color';

const mapClanRequest = (payload: Omit<Clan, 'id'>): Omit<ClanEdition, 'id'> => {
  return {
    ...payload,
    primaryColor: rgbHexColorToArgbInt(payload.primaryColor),
    secondaryColor: rgbHexColorToArgbInt(payload.secondaryColor),
  };
};

export const mapClanResponse = (payload: ClanEdition): Clan => {
  return {
    ...payload,
    primaryColor: argbIntToRgbHexColor(payload.primaryColor),
    secondaryColor: argbIntToRgbHexColor(payload.secondaryColor),
  };
};

// TODO: backend pagination/region query!
export const getClans = async () => {
  const clans = await get<ClanWithMemberCount<ClanEdition>[]>('/clans');
  return clans.map(c => ({
    ...c,
    clan: mapClanResponse(c.clan),
  }));
};

export const getFilteredClans = (
  clans: ClanWithMemberCount<Clan>[],
  region: Region,
  search: string
) => {
  const searchQuery = search.toLowerCase();
  return clans.filter(
    c =>
      c.clan.region === region &&
      (c.clan.tag.toLowerCase().includes(searchQuery) ||
        c.clan.name.toLowerCase().includes(searchQuery))
  );
};

export const createClan = async (clan: Omit<Clan, 'id'>) =>
  mapClanResponse(await post<ClanEdition>('/clans', mapClanRequest(clan)));

export const updateClan = async (clanId: number, clan: Clan) =>
  mapClanResponse(await put<ClanEdition>(`/clans/${clanId}`, mapClanRequest(clan)));

export const getClan = async (id: number) =>
  mapClanResponse(await get<ClanEdition>(`/clans/${id}`));

export const getClanMembers = async (id: number) => get<ClanMember[]>(`/clans/${id}/members`);

export const updateClanMember = async (clanId: number, memberId: number, role: ClanMemberRole) =>
  put<ClanMember>(`/clans/${clanId}/members/${memberId}`, { role });

export const kickClanMember = async (clanId: number, memberId: number) =>
  del(`/clans/${clanId}/members/${memberId}`);

export const inviteToClan = async (clanId: number, inviteeId: number) =>
  post<ClanInvitation>(`/clans/${clanId}/invitations`, { inviteeId });

export const getClanInvitations = async (
  clanId: number,
  types: ClanInvitationType[],
  statuses: ClanInvitationStatus[]
) => {
  const params = new URLSearchParams();
  types.forEach(t => params.append('type[]', t));
  statuses.forEach(s => params.append('status[]', s));
  return get<ClanInvitation[]>(`/clans/${clanId}/invitations?${params}`);
};

export const respondToClanInvitation = async (
  clanId: number,
  clanInvitationId: number,
  accept: boolean
) => put<ClanInvitation>(`/clans/${clanId}/invitations/${clanInvitationId}/response`, { accept });

// TODO: need a name
export const getClanMember = (clanMembers: ClanMember[], userId: number) =>
  clanMembers.find(m => m.user.id === userId) || null;

export const canManageApplicationsValidate = (role: ClanMemberRole) =>
  [ClanMemberRole.Leader, ClanMemberRole.Officer].includes(role);

export const canUpdateClanValidate = (role: ClanMemberRole) =>
  [ClanMemberRole.Leader].includes(role);

export const canUpdateMemberValidate = (role: ClanMemberRole) =>
  [ClanMemberRole.Leader].includes(role);

export const canKickMemberValidate = (
  selfMember: ClanMember,
  member: ClanMember,
  clanMembersCount: number
) => {
  if (
    member.user.id === selfMember.user.id &&
    (member.role !== ClanMemberRole.Leader || clanMembersCount === 1)
  ) {
    return true;
  }

  return (
    (selfMember.role === ClanMemberRole.Leader &&
      [ClanMemberRole.Officer, ClanMemberRole.Member].includes(member.role)) ||
    (selfMember.role === ClanMemberRole.Officer && member.role === ClanMemberRole.Member)
  );
};

//
export const getClanArmory = (clanId: number) => {
  // TODO: GET
  return get<ClanArmoryItem[]>(`/clans/${clanId}/armory`);

  // const mock = [
  //   {
  //     item: {
  //       id: 'crpg_lion_imprinted_saber_h1',
  //       baseId: 'crpg_lion_imprinted_saber_h1',
  //       createdAt: '2023-07-13T21:43:44.0741909Z',
  //       rank: 1,
  //       name: 'Wooden Sword',
  //       culture: 'Neutral',
  //       type: 'OneHandedWeapon',
  //       price: 257,
  //       tier: 1.0245335,
  //       requirement: 0,
  //       weight: 1.42,
  //       flags: [],
  //       armor: null,
  //       mount: null,
  //       weapons: [
  //         {
  //           class: 'OneHandedSword',
  //           itemUsage: 'onehanded_block_shield_swing_thrust',
  //           accuracy: 0,
  //           missileSpeed: 0,
  //           stackAmount: 0,
  //           length: 95,
  //           balance: 0.75,
  //           handling: 95,
  //           bodyArmor: 1,
  //           flags: ['MeleeWeapon', 'NoBlood'],
  //           thrustDamage: 10,
  //           thrustDamageType: 'Blunt',
  //           thrustSpeed: 92,
  //           swingDamage: 10,
  //           swingDamageType: 'Blunt',
  //           swingSpeed: 92,
  //         },
  //       ],
  //     } as ClanArmoryItem['item'],
  //     owner: {
  //       id: 1,
  //       platform: 'Steam',
  //       platformUserId: '76561197987525637',
  //       name: 'takeo',
  //       avatar:
  //         'https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/2c/2ce4694f06523a2ffad501f5dc30ec7a8008e90e_full.jpg',
  //       region: 'Eu',
  //       clan: {
  //         id: 1,
  //         tag: 'PEC',
  //         primaryColor: '4278190318',
  //         secondaryColor: '4294957414',
  //         name: 'Pecores',
  //         bannerKey: '',
  //         region: 'Eu',
  //       },
  //     },
  //     borrower: null,
  //   },
  //   {
  //     item: {
  //       id: 'crpg_khuzait_lance_3_t5',
  //       baseId: 'crpg_khuzait_lance_3_t5',
  //       createdAt: '2023-07-13T21:43:44.0741909Z',
  //       rank: 0,
  //       name: 'Noble Cavalry Lance',
  //       culture: 'Khuzait',
  //       type: 'Polearm',
  //       price: 2593,
  //       tier: 3.7166684,
  //       requirement: 0,
  //       weight: 2.03,
  //       flags: ['NotStackable'],
  //       armor: null,
  //       mount: null,
  //       weapons: [
  //         {
  //           class: 'OneHandedPolearm',
  //           itemUsage: 'onehanded_polearm_block_long_rshield_thrust',
  //           accuracy: 0,
  //           missileSpeed: 0,
  //           stackAmount: 0,
  //           length: 200,
  //           balance: 0,
  //           handling: 62,
  //           bodyArmor: 0,
  //           flags: ['MeleeWeapon', 'WideGrip'],
  //           thrustDamage: 25,
  //           thrustDamageType: 'Pierce',
  //           thrustSpeed: 82,
  //           swingDamage: 0,
  //           swingDamageType: 'Undefined',
  //           swingSpeed: 18,
  //         },
  //         {
  //           class: 'TwoHandedPolearm',
  //           itemUsage: 'polearm_block_long_shield_thrust',
  //           accuracy: 0,
  //           missileSpeed: 0,
  //           stackAmount: 0,
  //           length: 200,
  //           balance: 0,
  //           handling: 58,
  //           bodyArmor: 0,
  //           flags: ['MeleeWeapon', 'NotUsableWithOneHand', 'WideGrip', 'TwoHandIdleOnMount'],
  //           thrustDamage: 25,
  //           thrustDamageType: 'Pierce',
  //           thrustSpeed: 91,
  //           swingDamage: 0,
  //           swingDamageType: 'Undefined',
  //           swingSpeed: 68,
  //         },
  //         {
  //           class: 'TwoHandedPolearm',
  //           itemUsage: 'polearm_couch',
  //           accuracy: 0,
  //           missileSpeed: 0,
  //           stackAmount: 0,
  //           length: 200,
  //           balance: 0,
  //           handling: 62,
  //           bodyArmor: 0,
  //           flags: ['MeleeWeapon', 'WideGrip'],
  //           thrustDamage: 25,
  //           thrustDamageType: 'Pierce',
  //           thrustSpeed: 82,
  //           swingDamage: 0,
  //           swingDamageType: 'Undefined',
  //           swingSpeed: 18,
  //         },
  //       ],
  //     },
  //     owner: {
  //       id: 2,
  //       platform: 'Steam',
  //       platformUserId: '76561197987525637',
  //       name: 'orle',
  //       avatar: 'https://avatars.steamstatic.com/d51d5155b1a564421c0b3fd5fb7eed7c4474e73d_full.jpg',
  //       region: 'Eu',
  //       clan: {
  //         id: 1,
  //         tag: 'PEC',
  //         primaryColor: '4278190318',
  //         secondaryColor: '4294957414',
  //         name: 'Pecores',
  //         bannerKey: '',
  //         region: 'Eu',
  //       },
  //     },
  //     borrower: null,
  //   },
  // ] as ClanArmoryItem[];

  // return Promise.resolve([...mock]);
  // return Promise.resolve([]);
};

export const addItemToClanArmory = (clanId: number, userItemId: number) =>
  post(`/clans/${clanId}/armory`, {
    userItemId,
  });

export const removeItemFromClanArmory = (clanId: number, userItemId: number) =>
  del(`/clans/${clanId}/armory/${userItemId}`);

export const borrowItemFromClanArmory = (clanId: number, userItemId: number) =>
  put(`/clans/${clanId}/armory/${userItemId}/borrow`);

export const returnItemToClanArmory = (clanId: number, userItemId: number) =>
  put(`/clans/${clanId}/armory/${userItemId}/return`);
