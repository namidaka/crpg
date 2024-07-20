using AutoMapper;
using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Notification;

namespace Crpg.Application.ActivityLogs.Models;

public record UserNotificationViewModel : IMapFrom<UserNotification>
{
    public int Id { get; init; }

    public NotificationState State { get; init; }

    public ActivityLogViewModel? Notification { get; init; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UserNotification, UserNotificationViewModel>()
            .ForMember(un => un.Notification, opt => opt.MapFrom(src => src.ActivityLog));
    }
}
