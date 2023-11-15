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
  ServerJoined = 'ServerJoined',
  ChatMessageSent = 'ChatMessageSent',
  TeamHit = 'TeamHit',
  ClanArmoryAddItem = 'ClanArmoryAddItem',
  ClanArmoryRemoveItem = 'ClanArmoryRemoveItem',
  ClanArmoryReturnItem = 'ClanArmoryReturnItem',
  ClanArmoryBorrowItem = 'ClanArmoryBorrowItem',
}

export interface ActivityLog {
  id: number;
  type: ActivityLogType;
  userId: number;
  metadata: Record<string, string>;
  createdAt: Date;
}
