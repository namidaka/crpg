using AutoMapper;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Application.Items.Models;
using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Clans.Commands;

public record ArmoryReturnCommand : IMediatorRequest
{
    public int UserItemId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<ArmoryReturnCommand>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<ArmoryReturnCommand>();

        private readonly ICrpgDbContext _db;

        public Handler(ICrpgDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(ArmoryReturnCommand req, CancellationToken cancellationToken)
        {
            var borrow = await _db.ArmoryBorrows
                 .Include(e => e.UserItem)
                 .FirstOrDefaultAsync(e => e.UserItemId == req.UserItemId && e.UserId == req.UserId, cancellationToken);
            if (borrow == null)
            {
                return new(CommonErrors.UserItemNotFound(req.UserId));
            }

            if (borrow.UserItem!.IsBroken)
            {
                return new(CommonErrors.ItemBroken(borrow.UserItem.ItemId));
            }

            _db.ArmoryBorrows.Remove(borrow);

            await _db.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("User '{0}' returned item '{2}' to the armory '{3}'", req.UserId, req.UserItemId, borrow.ClanId);

            return Result.NoErrors;
        }
    }
}
