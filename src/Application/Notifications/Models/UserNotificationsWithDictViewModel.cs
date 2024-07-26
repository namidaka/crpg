using System.Collections;
using Crpg.Application.Clans.Models;
using Crpg.Application.Common.Mappings;
using Crpg.Application.Users.Models;
using Crpg.Domain.Entities.Notification;

namespace Crpg.Application.ActivityLogs.Models;

public record UserNotificationsWithDictViewModel
{
    public IList<UserNotificationViewModel> Notifications { get; init; } = Array.Empty<UserNotificationViewModel>();
    public IList<ClanPublicViewModel> Clans { get; init; } = Array.Empty<ClanPublicViewModel>();
    public IList<UserPublicViewModel> Users { get; init; } = Array.Empty<UserPublicViewModel>();
}
