using AutoMapper;
using Crpg.Application.Characters.Models;
using Crpg.Application.Clans.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Notifications.Models;
using Crpg.Application.Users.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Notifications.Queries;

public record GetUserNotificationsQuery : IMediatorRequest<UserNotificationsWithDictViewModel>
{
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<GetUserNotificationsQuery, UserNotificationsWithDictViewModel>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // TODO: all + pagination/filters (by date? by type?)
        public async Task<Result<UserNotificationsWithDictViewModel>> Handle(GetUserNotificationsQuery req,
            CancellationToken cancellationToken)
        {
            var userNotifications = await _db.UserNotifications
                .Include(un => un.ActivityLog)
                    .ThenInclude(al => al!.Metadata)
                .Where(un => un.UserId == req.UserId)
                .OrderByDescending(un => un.CreatedAt)
                .Take(1000) // TODO:
                .ToArrayAsync(cancellationToken);

            // TODO: to fns: extract from activityLog.metaData
            IList<int> clansIds = new List<int>();
            IList<int> usersIds = new List<int>();
            IList<int> charactersIds = new List<int>();

            foreach (var un in userNotifications)
            {
                usersIds.Add(un.ActivityLog!.UserId);

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

                    if (md.Key == "characterId")
                    {
                        charactersIds.Add(Convert.ToInt32(md.Value));
                    }
                }
            }

            var clans = await _db.Clans.Where(c => clansIds.Contains(c.Id)).ToArrayAsync();
            var users = await _db.Users.Where(u => usersIds.Contains(u.Id)).ToArrayAsync();
            var characters = await _db.Characters.Where(c => charactersIds.Contains(c.Id)).ToArrayAsync();

            UserNotificationsWithDictViewModel dd = new()
            {
                Notifications = _mapper.Map<IList<UserNotificationViewModel>>(userNotifications),
                Clans = _mapper.Map<IList<ClanPublicViewModel>>(clans),
                Users = _mapper.Map<IList<UserPublicViewModel>>(users),
                Characters = _mapper.Map<IList<CharacterPublicViewModel>>(characters),
            };
            return new(dd);
        }
    }
}
