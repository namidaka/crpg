namespace Crpg.Domain.Entities.ActivityLogs;

public enum ActivityLogType
{
    UserCreated,
    UserDeleted,
    UserRenamed,
    UserRewarded,
    ItemBought,
    ItemSold,
    ItemBroke,
    ItemReforged,
    ItemRepaired,
    ItemUpgraded,
    ItemReturned,
    CharacterCreated,
    CharacterDeleted,
    CharacterRatingReset,
    CharacterRespecialized,
    CharacterRetired,
    CharacterRewarded,
    CharacterEarned,
    ServerJoined,
    ChatMessageSent,
    TeamHit,
    ClanCreated, // TODO:
    ClanDeleted, // TODO:
    ClanInvitationCreated,
    ClanInvitationDeclined,
    ClanInvitationAccepted,
    ClanRoleEdited, // TODO:
    ClanArmoryAddItem,
    ClanArmoryRemoveItem,
    ClanArmoryReturnItem,
    ClanArmoryBorrowItem,
}
