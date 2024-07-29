using Crpg.Domain.Entities.Notification;

namespace Crpg.Application.Common.Services;

internal interface IUserNotificationService
{
    UserNotification CreateItemReturnedToUser(int userId, int activityLogId);
    UserNotification CreateClanInvitationCreatedToUser(int userId, int activityLogId);
    UserNotification CreateClanMemberRoleChangedToUser(int userId, int activityLogId);
    UserNotification CreateClanMemberLeavedToClanLeader(int userId, int activityLogId);
    UserNotification CreateClanMemberKickedToExClanMember(int userId, int activityLogId);
    UserNotification CreateClanInvitationCreatedToClanOfficers(int userId, int activityLogId);
    UserNotification CreateClanInvitationDeclinedToUser(int userId, int activityLogId);
    UserNotification CreateClanInvitationAcceptedToUser(int userId, int activityLogId);
    UserNotification CreateUserRewardedToUser(int userId, int activityLogId);
}

internal class UserNotificationService : IUserNotificationService
{
    public UserNotification CreateItemReturnedToUser(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ItemReturned, userId, activityLogId);
    }

    public UserNotification CreateClanMemberRoleChangedToUser(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanMemberRoleChangedToUser, userId, activityLogId);
    }

    public UserNotification CreateClanMemberLeavedToClanLeader(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanMemberLeavedToClanLeader, userId, activityLogId);
    }

    public UserNotification CreateClanMemberKickedToExClanMember(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanMemberKickedToExClanMember, userId, activityLogId);
    }

    public UserNotification CreateClanInvitationCreatedToUser(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanInvitationCreatedToUser, userId, activityLogId);
    }

    public UserNotification CreateClanInvitationCreatedToClanOfficers(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanInvitationCreatedToClanOfficers, userId, activityLogId);
    }

    public UserNotification CreateClanInvitationDeclinedToUser(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanInvitationDeclinedToUser, userId, activityLogId);
    }

    public UserNotification CreateClanInvitationAcceptedToUser(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanInvitationAcceptedToUser, userId, activityLogId);
    }

    public UserNotification CreateUserRewardedToUser(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.UserRewardedToUser, userId, activityLogId);
    }

    private UserNotification CreateLog(NotificationType type, int userId, int activityLogId)
    {
        return new UserNotification
        {
            Type = type,
            UserId = userId,
            ActivityLogId = activityLogId,
        };
    }
}
