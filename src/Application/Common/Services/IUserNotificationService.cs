using Crpg.Domain.Entities.Notification;

namespace Crpg.Application.Common.Services;

internal interface IUserNotificationService
{
    UserNotification CreateItemReturnedToUser(int userId, int activityLogId);
    UserNotification CreateClanApplicationCreatedToUser(int userId, int activityLogId);
    UserNotification CreateClanApplicationCreatedToClanOfficers(int userId, int activityLogId);
    UserNotification CreateClanApplicationAcceptedToUser(int userId, int activityLogId);
    UserNotification CreateClanApplicationDeclinedToUser(int userId, int activityLogId);
    UserNotification CreateClanMemberRoleChangedToUser(int userId, int activityLogId);
    UserNotification CreateClanMemberLeavedToClanLeader(int userId, int activityLogId);
    UserNotification CreateClanMemberKickedToExClanMember(int userId, int activityLogId);
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

    public UserNotification CreateClanApplicationCreatedToUser(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanApplicationCreatedToUser, userId, activityLogId);
    }

    public UserNotification CreateClanApplicationCreatedToClanOfficers(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanApplicationCreatedToClanOfficers, userId, activityLogId);
    }

    public UserNotification CreateClanApplicationAcceptedToUser(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanApplicationAcceptedToUser, userId, activityLogId);
    }

    public UserNotification CreateClanApplicationDeclinedToUser(int userId, int activityLogId)
    {
        return CreateLog(NotificationType.ClanApplicationDeclinedToUser, userId, activityLogId);
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
