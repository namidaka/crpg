using AutoMapper;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Notifications.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.ActivityLogs.Commands;

public record UpdateUserNotificationCommand : IMediatorRequest<GetUserNotificationsQuery>
{
    public int UserNotificationId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequest<UpdateUserNotificationCommand>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<UpdateUserNotificationCommand>();

        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<GetUserNotificationsQuery>> Handle(UpdateUserNotificationCommand req, CancellationToken cancellationToken)
        {
            var userNotification = await _db.UserNotifications
               .FirstOrDefaultAsync(un => un.Id == req.UserNotificationId && un.UserId == req.UserId, cancellationToken);

            if (userNotification == null)
            {
                return new(CommonErrors.UserNotificationNotFound(req.UserId, req.UserNotificationId));
            }

            userNotification.State = Domain.Entities.Notification.NotificationState.Read;

            await _db.SaveChangesAsync(cancellationToken);
            Logger.LogInformation("User '{0}' updated the notification '{1}'", req.UserId, req.UserNotificationId);
            return new(_mapper.Map<GetUserNotificationsQuery>(userNotification));
        }
    }
}
