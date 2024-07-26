using AutoMapper;
using Crpg.Application.ActivityLogs.Models;
using Crpg.Application.Clans.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Users.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Notifications.Queries;

public record GetUserNotificationsQuery : IMediatorRequest<UserNotificationsWithDictViewModel>
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
    //     }
    // }

    internal class Handler : IMediatorRequestHandler<GetUserNotificationsQuery, UserNotificationsWithDictViewModel>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // TODO: last 5
        // TODO: all + pagination/filters (by date? by type?)
        public async Task<Result<UserNotificationsWithDictViewModel>> Handle(GetUserNotificationsQuery req,
            CancellationToken cancellationToken)
        {
            var userNotifications = await _db.UserNotifications
                .Include(un => un.ActivityLog)
                    .ThenInclude(al => al!.Metadata)
                .Where(un => un.UserId == req.UserId)
                // l.CreatedAt >= req.From
                // && l.CreatedAt <= req.To
                .OrderByDescending(l => l.CreatedAt)
                .Take(1000) // TODO:
                .ToArrayAsync(cancellationToken);

            // TODO: to fns
            IList<int> clansIds = new List<int>();
            IList<int> usersIds = new List<int>();
            foreach (var un in userNotifications)
            {
                foreach (var md in un.ActivityLog!.Metadata)
                {
                    if (md.Key == "clanId")
                    {
                        clansIds.Add(Convert.ToInt32(md.Value));
                    }

                    if (md.Key == "userId" || md.Key == "actorUserId")
                    {
                        usersIds.Add(Convert.ToInt32(md.Value));
                    }
                }
            }

            var clans = await _db.Clans.Where(c => clansIds.Contains(c.Id)).ToArrayAsync();
            var users = await _db.Users.Where(c => usersIds.Contains(c.Id)).ToArrayAsync();

            UserNotificationsWithDictViewModel dd = new()
            {
                Notifications = _mapper.Map<IList<UserNotificationViewModel>>(userNotifications),
                Clans = _mapper.Map<IList<ClanPublicViewModel>>(clans),
                Users = _mapper.Map<IList<UserPublicViewModel>>(users),
            };
            return new(dd);
        }
    }
}
