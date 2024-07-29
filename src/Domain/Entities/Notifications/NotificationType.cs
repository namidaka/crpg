namespace Crpg.Domain.Entities.Notification;

public enum NotificationType
{
    UserRewardedToUser,

    ItemReturned,

    ClanInvitationCreatedToUser,
    ClanInvitationCreatedToClanOfficers,
    ClanInvitationDeclinedToUser,
    ClanInvitationAcceptedToUser,
    ClanMemberRoleChangedToUser,
    ClanMemberLeavedToClanLeader,
    ClanMemberKickedToExClanMember,
}
