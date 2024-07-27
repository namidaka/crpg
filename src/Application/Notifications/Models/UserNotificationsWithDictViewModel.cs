using Crpg.Application.Clans.Models;
using Crpg.Application.Users.Models;

namespace Crpg.Application.Notifications.Models;

public record UserNotificationsWithDictViewModel
{
    public IList<UserNotificationViewModel> Notifications { get; init; } = Array.Empty<UserNotificationViewModel>();
    public IList<ClanPublicViewModel> Clans { get; init; } = Array.Empty<ClanPublicViewModel>();
    public IList<UserPublicViewModel> Users { get; init; } = Array.Empty<UserPublicViewModel>();
}
