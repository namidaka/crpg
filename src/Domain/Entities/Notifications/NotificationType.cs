namespace Crpg.Domain.Entities.Notification;

public enum NotificationType
{
    UserRewardedToUser,

    ItemReturned,

    ClanApplicationCreatedToUser,
    ClanApplicationCreatedToClanOfficers,
    ClanApplicationAcceptedToUser,
    ClanApplicationDeclinedToUser,
    ClanMemberRoleChangedToUser,
    ClanMemberLeavedToClanLeader,
    ClanMemberKickedToExClanMember,
}
