namespace Crpg.Domain.Entities.Notification;

public enum NotificationType
{
    UserRewardedToUser,
    CharacterRewardedToUser,
    ItemReturned,
    ClanApplicationCreatedToUser,
    ClanApplicationCreatedToClanOfficers,
    ClanApplicationAcceptedToUser,
    ClanApplicationDeclinedToUser,
    ClanMemberRoleChangedToUser,
    ClanMemberLeavedToClanLeader,
    ClanMemberKickedToExClanMember,
}
