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

public record AddClanArmoryCommand : IMediatorRequest<ClanArmoryItemViewModel>
{
    public int UserId { get; init; }
    public int ClanId { get; init; }
    public int UserItemId { get; init; }

    internal class Handler : IMediatorRequestHandler<AddClanArmoryCommand, ClanArmoryItemViewModel>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<AddClanArmoryCommand>();

        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;
        private readonly IClanService _clanService;

        public Handler(ICrpgDbContext db, IMapper mapper, IClanService clanService)
        {
            _db = db;
            _mapper = mapper;
            _clanService = clanService;
        }

        public async Task<Result<ClanArmoryItemViewModel>> Handle(AddClanArmoryCommand req, CancellationToken cancellationToken)
        {
            var user = await _db.Users.AsNoTracking()
                .Include(e => e.ClanMembership)
                .FirstOrDefaultAsync(e => e.Id == req.UserId, cancellationToken);
            if (user == null)
            {
                return new(CommonErrors.UserNotFound(req.UserId));
            }

            var error = _clanService.CheckClanMembership(user, req.ClanId);
            if (error != null)
            {
                return new(error);
            }

            var userItem = await _db.UserItems.AsNoTracking()
                .Include(e => e.User!).ThenInclude(e => e.ClanMembership)
                .Include(e => e.Item)
                .Include(e => e.EquippedItems)
                .Include(e => e.ClanArmoryItem)
                .FirstOrDefaultAsync(e => e.UserId == req.UserId && e.Id == req.UserItemId, cancellationToken);
            if (userItem == null)
            {
                return new(CommonErrors.UserItemNotFound(req.UserItemId));
            }

            if (userItem.IsBroken)
            {
                return new(CommonErrors.ItemBroken(userItem.ItemId));
            }

            if (userItem.EquippedItems.Any())
            {
                return new(CommonErrors.ClanArmoryItemBusy(userItem.UserId));
            }

            if (userItem.ClanArmoryItem != null)
            {
                return new(CommonErrors.ClanArmoryItemBusy(userItem.UserId));
            }

            var clan = await _db.Clans
                .Include(e => e.ArmoryItems)
                .FirstOrDefaultAsync(e => e.Id == req.ClanId);
            if (clan == null)
            {
                return new(CommonErrors.ClanNotFound(req.ClanId));
            }

            if (userItem.Item!.Rank < clan.ArmoryMinRank)
            {
                return new(CommonErrors.UserItemMinRank(userItem.Id, userItem.Item.Rank));
            }

            ClanArmoryItem armoryItem = new() { UserItemId = req.UserItemId, ClanId = req.ClanId, UserItem = userItem };
            clan.ArmoryItems.Add(armoryItem);
            await _db.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("User '{0}' added item '{1}' to the armory '{2}'", req.UserId, req.UserItemId, req.ClanId);

            return new(_mapper.Map<ClanArmoryItemViewModel>(armoryItem));
        }
    }
}
