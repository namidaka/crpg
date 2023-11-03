using AutoMapper;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Clans.Commands;

public record ArmoryRemoveCommand : IMediatorRequest
{
    public int UserItemId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<ArmoryRemoveCommand>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<ArmoryRemoveCommand>();

        private readonly ICrpgDbContext _db;

        public Handler(ICrpgDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(ArmoryRemoveCommand req, CancellationToken cancellationToken)
        {
            var userItem = await _db.UserItems
                .Include(e => e.ArmoryItem)
                .FirstOrDefaultAsync(e => e.Id == req.UserItemId && e.UserId == req.UserId, cancellationToken);
            if (userItem == null || userItem.ArmoryItem == null)
            {
                return new(CommonErrors.UserItemNotFound(req.UserItemId));
            }

            _db.ArmoryItems.Remove(userItem.ArmoryItem);

            await _db.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("User '{0}' removed item '{2}' from the armory", req.UserId, req.UserItemId);

            return Result.NoErrors;
        }
    }
}
