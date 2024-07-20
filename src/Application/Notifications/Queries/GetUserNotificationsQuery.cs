using AutoMapper;
using Crpg.Application.ActivityLogs.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Domain.Entities.ActivityLogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Notifications.Queries;

public record GetUserNotificationsQuery : IMediatorRequest<IList<UserNotificationViewModel>>
{
    // public DateTime From { get; init; }
    // public DateTime To { get; init; }
    public int UserId { get; init; }
    // public ActivityLogType[] Types { get; init; } = Array.Empty<ActivityLogType>();

    // public class Validator : AbstractValidator<GetUserNotificationsQuery>
    // {
    //     public Validator()
    //     {
    //         RuleFor(l => l.From).LessThan(l => l.To);
    //         RuleForEach(l => l.Types).IsInEnum();
    //     }
    // }

    internal class Handler : IMediatorRequestHandler<GetUserNotificationsQuery, IList<UserNotificationViewModel>>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<IList<UserNotificationViewModel>>> Handle(GetUserNotificationsQuery req,
            CancellationToken cancellationToken)
        {
            var userNotifications = await _db.UserNotifications
                .Include(un => un.ActivityLog)
                    .ThenInclude(al => al!.Metadata)
                .Where(un => un.UserId == req.UserId)
                     // l.CreatedAt >= req.From
                     // && l.CreatedAt <= req.To
                    // && (req.Types.Length == 0 || req.Types.Contains(l.Type))
                .OrderByDescending(l => l.CreatedAt)
                .Take(1000)
                .ToArrayAsync(cancellationToken);
            return new(_mapper.Map<IList<UserNotificationViewModel>>(userNotifications));
        }
    }
}
