using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Clans.Commands;

public record RemoveClanArmoryCommand : IMediatorRequest
{
    public int UserId { get; init; }
    public int ClanId { get; init; }
    public int UserItemId { get; init; }

    internal class Handler : IMediatorRequestHandler<RemoveClanArmoryCommand>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<RemoveClanArmoryCommand>();

        private readonly ICrpgDbContext _db;

        public Handler(ICrpgDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(RemoveClanArmoryCommand req, CancellationToken cancellationToken)
        {
            var userItem = await _db.UserItems
                .Include(e => e.ClanArmoryItem)
                .FirstOrDefaultAsync(e => e.Id == req.UserItemId && e.UserId == req.UserId, cancellationToken);
            if (userItem == null || userItem.ClanArmoryItem == null)
            {
                return new(CommonErrors.UserItemNotFound(req.UserItemId));
            }

            _db.ClanArmoryItems.Remove(userItem.ClanArmoryItem);

            await _db.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("User '{0}' removed item '{1}' from the armory", req.UserId, req.UserItemId);

            return Result.NoErrors;
        }
    }
}
