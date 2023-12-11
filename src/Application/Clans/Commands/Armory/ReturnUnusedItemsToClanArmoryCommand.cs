using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Clans.Commands.Armory;

public record ReturnUnusedItemsToClanArmoryCommand : IMediatorRequest
{
    public TimeSpan Timeout { get; init; }

    internal class Handler : IMediatorRequestHandler<ReturnUnusedItemsToClanArmoryCommand>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<ReturnUnusedItemsToClanArmoryCommand>();

        private readonly ICrpgDbContext _db;

        public Handler(ICrpgDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(ReturnUnusedItemsToClanArmoryCommand req, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var users = await _db.Users
                .AsSplitQuery()
                .Include(u => u.ClanMembership!)
                    .ThenInclude(cm => cm.ArmoryBorrowedItems)
                    .ThenInclude(bi => bi.UserItem!)
                    .ThenInclude(ui => ui.EquippedItems)
                .Where(bi => bi.ClanMembership!.ArmoryBorrowedItems.Count > 0 && (now - bi.UpdatedAt) > req.Timeout)
                .ToArrayAsync(cancellationToken);

            foreach (var u in users)
            {
                var equipped = u.ClanMembership!.ArmoryBorrowedItems.SelectMany(bi => bi.UserItem!.EquippedItems);
                _db.EquippedItems.RemoveRange(equipped);
                _db.ClanArmoryBorrowedItems.RemoveRange(u.ClanMembership!.ArmoryBorrowedItems);
            }

            await _db.SaveChangesAsync(cancellationToken);
            Logger.LogInformation("Return unused items");

            return Result.NoErrors;
        }
    }
}
