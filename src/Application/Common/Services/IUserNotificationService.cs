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
    UserNotification CreateCharacterRewardedToUser(int userId, int activityLogId);
}

internal class UserNotificationService : IUserNotificationService
{
    public UserNotification CreateItemReturnedToUser(int userId, int activityLogId)
    {
        return CreateNotification(NotificationType.ItemReturned, userId, activityLogId);
    }

    public UserNotification CreateClanMemberRoleChangedToUser(int userId, int activityLogId)
    {
        return CreateNotification(NotificationType.ClanMemberRoleChangedToUser, userId, activityLogId);
    }

    public UserNotification CreateClanMemberLeavedToClanLeader(int userId, int activityLogId)
    {
        return CreateNotification(NotificationType.ClanMemberLeavedToClanLeader, userId, activityLogId);
    }

    public UserNotification CreateClanMemberKickedToExClanMember(int userId, int activityLogId)
    {
        return CreateNotification(NotificationType.ClanMemberKickedToExClanMember, userId, activityLogId);
    }

    public UserNotification CreateClanApplicationCreatedToUser(int userId, int activityLogId)
    {
        return CreateNotification(NotificationType.ClanApplicationCreatedToUser, userId, activityLogId);
    }

    public UserNotification CreateClanApplicationCreatedToClanOfficers(int userId, int activityLogId)
    {
        return CreateNotification(NotificationType.ClanApplicationCreatedToClanOfficers, userId, activityLogId);
    }

    public UserNotification CreateClanApplicationAcceptedToUser(int userId, int activityLogId)
    {
        return CreateNotification(NotificationType.ClanApplicationAcceptedToUser, userId, activityLogId);
    }

    public UserNotification CreateClanApplicationDeclinedToUser(int userId, int activityLogId)
    {
        return CreateNotification(NotificationType.ClanApplicationDeclinedToUser, userId, activityLogId);
    }

    public UserNotification CreateUserRewardedToUser(int userId, int activityLogId)
    {
        return CreateNotification(NotificationType.UserRewardedToUser, userId, activityLogId);
    }

    public UserNotification CreateCharacterRewardedToUser(int userId, int activityLogId)
    {
        return CreateNotification(NotificationType.CharacterRewardedToUser, userId, activityLogId);
    }

    private UserNotification CreateNotification(NotificationType type, int userId, int activityLogId)
    {
        return new UserNotification
        {
            Type = type,
            UserId = userId,
            ActivityLogId = activityLogId,
        };
    }
}
