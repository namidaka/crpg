using AutoMapper;
using Crpg.Application.Clans.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Clans.Commands;

public record ArmoryBorrowCommand : IMediatorRequest<ArmoryBorrowViewModel>
{
    public int UserItemId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<ArmoryBorrowCommand, ArmoryBorrowViewModel>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<ArmoryBorrowCommand>();

        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<ArmoryBorrowViewModel>> Handle(ArmoryBorrowCommand req, CancellationToken cancellationToken)
        {
            var user = await _db.Users.AsNoTracking()
                .Include(e => e.Items)
                .Include(e => e.ClanMembership)
                .FirstOrDefaultAsync(e => e.Id == req.UserId, cancellationToken);
            if (user == null)
            {
                return new(CommonErrors.UserNotFound(req.UserId));
            }

            if (user.ClanMembership == null)
            {
                return new(CommonErrors.UserNotInAClan(req.UserId));
            }

            var userItem = await _db.UserItems.AsNoTracking()
                .Include(e => e.ArmoryItem!)
                    .ThenInclude(e => e.Borrow)
                .FirstOrDefaultAsync(e => e.Id == req.UserItemId, cancellationToken);
            if (userItem == null || userItem.ArmoryItem == null)
            {
                return new(CommonErrors.UserItemNotFound(req.UserItemId));
            }

            if (userItem.ArmoryItem.Borrow != null)
            {
                return new(CommonErrors.ArmoryItemBusy(req.UserItemId));
            }

            if (user.Items.Any(e => e.ItemId == userItem.ItemId))
            {
                return new(CommonErrors.ItemAlreadyOwned(userItem.ItemId));
            }

            var clan = await _db.Clans
                .Include(e => e.ArmoryBorrows)
                .Include(e => e.ArmoryItems)
                .FirstOrDefaultAsync(e => e.Id == user.ClanMembership.ClanId, cancellationToken);
            if (clan == null)
            {
                return new(CommonErrors.ClanNotFound(user.ClanMembership.ClanId));
            }

            var borrow = new ArmoryBorrow { UserItemId = req.UserItemId, UserId = req.UserId };
            clan.ArmoryBorrows.Add(borrow);

            await _db.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("User '{0}' borrowed item '{2}' from the armory '{3}'", req.UserId, req.UserItemId, user.ClanMembership.ClanId);

            return new(_mapper.Map<ArmoryBorrowViewModel>(borrow));
        }
    }
}
