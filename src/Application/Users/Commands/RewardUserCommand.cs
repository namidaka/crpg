using AutoMapper;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Application.Users.Models;
using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Users.Commands;

public record RewardUserCommand : IMediatorRequest<UserViewModel>
{
    public int UserId { get; init; }
    public int Gold { get; init; }
    public int HeirloomPoints { get; init; }
    public string ItemId { get; init; } = string.Empty;

    internal class Handler : IMediatorRequestHandler<RewardUserCommand, UserViewModel>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<RewardUserCommand>();

        private readonly IActivityLogService _activityLogService;
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(IActivityLogService activityLogService, ICrpgDbContext db, IMapper mapper)
        {
            _activityLogService = activityLogService;
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<UserViewModel>> Handle(RewardUserCommand req, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == req.UserId, cancellationToken);
            if (user == null)
            {
                return new(CommonErrors.UserNotFound(req.UserId));
            }

            user.Gold = Math.Max(user.Gold + req.Gold, 0);
            user.HeirloomPoints = Math.Max(user.HeirloomPoints + req.HeirloomPoints, 0);

            _db.ActivityLogs.Add(_activityLogService.CreateUserRewardedLog(req.UserId, req.Gold, req.HeirloomPoints));

            if (req.ItemId != string.Empty)
            {
                var currentItem = await _db.Items.FirstOrDefaultAsync(i => i.Id == req.ItemId, cancellationToken);
                if (currentItem == null)
                {
                    return new(CommonErrors.ItemNotFound(req.ItemId));
                }

                var items = await _db.Items.Where(i => i.BaseId == currentItem.BaseId).ToArrayAsync(cancellationToken);

                var existingPersonalItems = await _db.PersonalItems
                    .Where(pi => pi.UserId == req.UserId)
                    .ToDictionaryAsync(i => i.ItemId, cancellationToken);

                foreach (var item in items)
                {
                    var personalItem = new PersonalItem
                    {
                        UserId = req.UserId,
                        Item = item,
                    };

                    if (!existingPersonalItems.ContainsKey(item.Id))
                    {
                        user.PersonalItems.Add(personalItem);
                    }
                }

                var userItem = new UserItem
                {
                    UserId = req.UserId,
                    Item = currentItem,
                };
                user.Items.Add(userItem);
            }

            await _db.SaveChangesAsync(cancellationToken);
            Logger.LogInformation("User '{0}' rewarded", req.UserId);

            return new Result<UserViewModel>(_mapper.Map<UserViewModel>(user));
        }
    }
}
