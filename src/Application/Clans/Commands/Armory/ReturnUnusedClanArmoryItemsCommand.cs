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

namespace Crpg.Application.Clans.Commands.Armory;

public record ReturnUnusedClanArmoryItemsCommand : IMediatorRequest
{
    public TimeSpan Timeout { get; init; }

    internal class Handler : IMediatorRequestHandler<ReturnUnusedClanArmoryItemsCommand>
    {
        private static readonly ILogger Logger = LoggerFactory.CreateLogger<ReturnUnusedClanArmoryItemsCommand>();

        private readonly ICrpgDbContext _db;
        private readonly IClanService _clanService;

        public Handler(ICrpgDbContext db, IClanService clanService)
        {
            _db = db;
            _clanService = clanService;
        }

        public async Task<Result> Handle(ReturnUnusedClanArmoryItemsCommand req, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var users = await _db.Users
                .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows).ThenInclude(e => e.UserItem!).ThenInclude(e => e.EquippedItems)
                .Where(e => e.ClanMembership!.ArmoryBorrows.Count > 0 && (now - e.UpdatedAt) > req.Timeout)
                .ToArrayAsync(cancellationToken);

            foreach (var u in users)
            {
                var equipped = u.ClanMembership!.ArmoryBorrows.SelectMany(e => e.UserItem!.EquippedItems);
                _db.EquippedItems.RemoveRange(equipped);
                _db.ClanArmoryBorrows.RemoveRange(u.ClanMembership!.ArmoryBorrows);
            }

            await _db.SaveChangesAsync(cancellationToken);
            Logger.LogInformation("Return unused items");

            return Result.NoErrors;
        }
    }
}
