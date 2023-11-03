using AutoMapper;
using Crpg.Application.Clans.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Application.Items.Commands;
using Crpg.Application.Items.Models;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoggerFactory = Crpg.Logging.LoggerFactory;

namespace Crpg.Application.Clans.Commands;

public record ArmoryAddCommand : IMediatorRequest<ArmoryItemViewModel>
{
    public int UserItemId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<ArmoryAddCommand, ArmoryItemViewModel>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<ArmoryAddCommand>();

        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<ArmoryItemViewModel>> Handle(ArmoryAddCommand req, CancellationToken cancellationToken)
        {
            var userItem = await _db.UserItems.AsNoTracking()
                .Include(e => e.User!)
                    .ThenInclude(e => e.ClanMembership)
                .Include(e => e.Item)
                .Include(e => e.EquippedItems)
                .Include(e => e.ArmoryItem)
                .FirstOrDefaultAsync(e => e.UserId == req.UserId && e.Id == req.UserItemId, cancellationToken);
            if (userItem == null)
            {
                return new(CommonErrors.UserItemNotFound(req.UserItemId));
            }

            if (userItem.IsBroken)
            {
                return new(CommonErrors.ItemBroken(userItem.ItemId));
            }

            if (userItem.User!.ClanMembership == null)
            {
                return new(CommonErrors.UserNotInAClan(req.UserId));
            }

            if (userItem.EquippedItems.Any())
            {
                return new(CommonErrors.ArmoryItemBusy(userItem.UserId));
            }

            if (userItem.ArmoryItem != null)
            {
                return new(CommonErrors.ArmoryItemBusy(userItem.UserId));
            }

            var clan = await _db.Clans
                .Include(e => e.ArmoryItems)
                .FirstOrDefaultAsync(e => e.Id == userItem.User.ClanMembership.ClanId);
            if (clan == null)
            {
                return new(CommonErrors.ClanNotFound(userItem.User.ClanMembership.ClanId));
            }

            if (userItem.Item!.Rank < clan.ArmoryMinRank)
            {
                return new(CommonErrors.UserItemMinRank(userItem.Id, userItem.Item.Rank));
            }

            var armoryItem = new ArmoryItem { UserItemId = req.UserItemId, ClanId = clan.Id };
            clan.ArmoryItems.Add(armoryItem);
            await _db.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("User '{0}' added item '{2}' to the armory '{3}'", req.UserId, req.UserItemId, clan.Id);

            return new(_mapper.Map<ArmoryItemViewModel>(armoryItem));
        }
    }
}
