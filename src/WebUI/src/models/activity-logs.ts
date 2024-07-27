export enum ActivityLogType {
  UserCreated = 'UserCreated',
  UserDeleted = 'UserDeleted',
  UserRenamed = 'UserRenamed',
  UserRewarded = 'UserRewarded',
  ItemBought = 'ItemBought',
  ItemSold = 'ItemSold',
  ItemBroke = 'ItemBroke',
  ItemRepaired = 'ItemRepaired',
  ItemUpgraded = 'ItemUpgraded',
  CharacterCreated = 'CharacterCreated',
  CharacterDeleted = 'CharacterDeleted',
  CharacterRespecialized = 'CharacterRespecialized',
  CharacterRetired = 'CharacterRetired',
  CharacterRewarded = 'CharacterRewarded',
  CharacterEarned = 'CharacterEarned',
  ServerJoined = 'ServerJoined',
  ChatMessageSent = 'ChatMessageSent',
  TeamHit = 'TeamHit',
  ClanInvitationCreated = 'ClanInvitationCreated',
  ClanInvitationAccepted = 'ClanInvitationAccepted',
  ClanInvitationDeclined = 'ClanInvitationDeclined',
  ClanArmoryAddItem = 'ClanArmoryAddItem',
  ClanArmoryRemoveItem = 'ClanArmoryRemoveItem',
  ClanArmoryReturnItem = 'ClanArmoryReturnItem',
  ClanArmoryBorrowItem = 'ClanArmoryBorrowItem',
}

// TODO:
export type ClanInvitationCreatedMetadata = {
  clanId: string;
};

export type CharacterEarnedMetadata = {
  characterId: string;
  gameMode: string;
  experience: string;
  gold: string;
};

export type ActivityLog<T = { [key: string]: string }> = {
  id: number;
  type: ActivityLogType;
  userId: number;
  createdAt: Date;
  metadata: T;
};
