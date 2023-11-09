using System.Security.Claims;
using Crpg.Application.Characters.Commands;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Results;
using Crpg.Domain.Entities.Clans;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Crpg.Application.Common.Services;

internal interface IClanService
{
    Task<Result<User>> GetClanMember(ICrpgDbContext db, int userId, int clanId, CancellationToken cancellationToken);
    Error? CheckClanMembership(User user, int clanId);
    Task<Result<ClanMember>> JoinClan(ICrpgDbContext db, User user, int clanId, CancellationToken cancellationToken);
    Task<Result> LeaveClan(ICrpgDbContext db, ClanMember member, CancellationToken cancellationToken);

    Task<Result<ClanArmoryItem>> AddArmoryItem(ICrpgDbContext db, Clan clan, User user, int userItemId, CancellationToken cancellationToken);
    Task<Result> RemoveArmoryItem(ICrpgDbContext db, Clan clan, User user, int userItemId, CancellationToken cancellationToken);
    Task<Result<ClanArmoryBorrow>> BorrowArmoryItem(ICrpgDbContext db, Clan clan, User user, int userItemId, CancellationToken cancellationToken);
    Task<Result> ReturnArmoryItem(ICrpgDbContext db, Clan clan, User user, int userItemId, CancellationToken cancellationToken);
}

internal class ClanService : IClanService
{
    public async Task<Result<User>> GetClanMember(ICrpgDbContext db, int userId, int clanId, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Include(u => u.ClanMembership)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return new(CommonErrors.UserNotFound(userId));
        }

        var error = CheckClanMembership(user, clanId);
        return error != null ? new(error) : new(user);
    }

    public Error? CheckClanMembership(User user, int clanId)
    {
        if (user.ClanMembership == null)
        {
            return CommonErrors.UserNotInAClan(user.Id);
        }

        if (user.ClanMembership.ClanId != clanId)
        {
            return CommonErrors.UserNotAClanMember(user.Id, clanId);
        }

        return null;
    }

    public async Task<Result<ClanMember>> JoinClan(ICrpgDbContext db, User user, int clanId, CancellationToken cancellationToken)
    {
        user.ClanMembership = new ClanMember
        {
            UserId = user.Id,
            ClanId = clanId,
            Role = ClanMemberRole.Member,
        };

        // Joining a clan declines all pending invitations and delete pending requests to join.
        var invitations = await db.ClanInvitations
            .Where(i => i.InviteeId == user.Id && i.Status == ClanInvitationStatus.Pending)
            .ToArrayAsync(cancellationToken);
        foreach (var invitation in invitations)
        {
            if (invitation.Type == ClanInvitationType.Request)
            {
                db.ClanInvitations.Remove(invitation);
            }
            else if (invitation.Type == ClanInvitationType.Offer)
            {
                invitation.Status = ClanInvitationStatus.Declined;
            }
        }

        return new(user.ClanMembership);
    }

    public async Task<Result> LeaveClan(ICrpgDbContext db, ClanMember member, CancellationToken cancellationToken)
    {
        // If user is leader and wants to leave, he needs to be the last member or have designated a new leader first.
        if (member.Role == ClanMemberRole.Leader)
        {
            await db.Entry(member)
                .Reference(m => m.Clan!)
                .Query()
                .Include(c => c.Members)
                .LoadAsync(cancellationToken);

            if (member.Clan!.Members.Count > 1)
            {
                return new Result(CommonErrors.ClanNeedLeader(member.ClanId));
            }

            db.Clans.Remove(member.Clan);
        }

        db.ClanMembers.Remove(member);
        return Result.NoErrors;
    }

    public async Task<Result<ClanArmoryItem>> AddArmoryItem(ICrpgDbContext db, Clan clan, User user, int userItemId, CancellationToken cancellationToken)
    {
        await db.Entry(user)
            .Reference(e => e.ClanMembership)
            .LoadAsync(cancellationToken);

        var errors = CheckClanMembership(user, clan.Id);
        if (errors != null)
        {
            return new(errors);
        }

        var userItem = await db.UserItems
                .Where(e => e.UserId == user.Id && e.Id == userItemId)
                .Include(e => e.Item)
                .Include(e => e.ClanArmoryItem)
                .Include(e => e.EquippedItems)
                .FirstOrDefaultAsync(cancellationToken);
        if (userItem == null)
        {
            return new(CommonErrors.UserItemNotFound(userItemId));
        }

        if (userItem.IsBroken)
        {
            return new(CommonErrors.ItemBroken(userItem.ItemId));
        }

        if (userItem.EquippedItems.Any())
        {
            return new(CommonErrors.ClanArmoryItemBusy(userItemId));
        }

        if (userItem.ClanArmoryItem != null)
        {
            return new(CommonErrors.ClanArmoryItemBusy(userItemId));
        }

        await db.Entry(clan)
            .Collection(e => e.ArmoryItems)
            .LoadAsync();

        if (userItem.Item!.Rank < clan.ArmoryMinRank)
        {
            return new(CommonErrors.UserItemMinRank(userItemId, userItem.Item.Rank));
        }

        var armoryItem = new ClanArmoryItem { UserItem = userItem };
        clan.ArmoryItems.Add(armoryItem);

        return new(armoryItem);
    }

    public async Task<Result> RemoveArmoryItem(ICrpgDbContext db, Clan clan, User user, int userItemId, CancellationToken cancellationToken)
    {
        await db.Entry(user)
            .Reference(e => e.ClanMembership)
            .LoadAsync(cancellationToken);

        var errors = CheckClanMembership(user, clan.Id);
        if (errors != null)
        {
            return new(errors);
        }

        var userItem = await db.UserItems
            .Where(e => e.Id == userItemId && e.UserId == user.Id)
            .Include(e => e.ClanArmoryItem)
            .Include(e => e.EquippedItems)
            .FirstOrDefaultAsync(cancellationToken);
        if (userItem == null || userItem.ClanArmoryItem == null)
        {
            return new(CommonErrors.UserItemNotFound(userItemId));
        }

        db.EquippedItems.RemoveRange(userItem.EquippedItems);
        db.ClanArmoryItems.Remove(userItem.ClanArmoryItem);

        return Result.NoErrors;
    }

    public async Task<Result<ClanArmoryBorrow>> BorrowArmoryItem(ICrpgDbContext db, Clan clan, User user, int userItemId, CancellationToken cancellationToken)
    {
        await db.Entry(user)
            .Reference(e => e.ClanMembership)
            .LoadAsync();

        await db.Entry(user)
            .Collection(e => e.Items)
            .LoadAsync();

        var errors = CheckClanMembership(user, clan.Id);
        if (errors != null)
        {
            return new(errors);
        }

        var armoryItem = await db.ClanArmoryItems.AsNoTracking()
            .Where(e => e.UserItemId == userItemId && e.ClanId == clan.Id)
            .Include(e => e.UserItem)
            .Include(e => e.Borrow)
            .FirstOrDefaultAsync(cancellationToken);
        if (armoryItem == null)
        {
            return new(CommonErrors.UserItemNotFound(userItemId));
        }

        if (armoryItem.Borrow != null)
        {
            return new(CommonErrors.ClanArmoryItemBusy(userItemId));
        }

        if (user.Items.Any(e => e.ItemId == armoryItem.UserItem!.ItemId))
        {
            return new(CommonErrors.ItemAlreadyOwned(armoryItem.UserItem!.ItemId));
        }

        var borrow = new ClanArmoryBorrow { ClanId = clan.Id, UserItemId = userItemId, UserId = user.Id };
        db.ClanArmoryBorrows.Add(borrow);

        return new(borrow);
    }

    public async Task<Result> ReturnArmoryItem(ICrpgDbContext db, Clan clan, User user, int userItemId, CancellationToken cancellationToken)
    {
        await db.Entry(user)
            .Reference(e => e.ClanMembership)
            .LoadAsync();

        var errors = CheckClanMembership(user, clan.Id);
        if (errors != null)
        {
            return new(errors);
        }

        var borrow = await db.ClanArmoryBorrows
            .Where(e => e.UserItemId == userItemId && e.UserId == user.Id && e.ClanId == clan.Id)
            .Include(e => e.UserItem!).ThenInclude(e => e.EquippedItems)
            .FirstOrDefaultAsync(cancellationToken);
        if (borrow == null)
        {
            return new(CommonErrors.UserItemNotFound(userItemId));
        }

        if (borrow.UserItem!.IsBroken)
        {
            return new(CommonErrors.ItemBroken(borrow.UserItem.ItemId));
        }

        db.EquippedItems.RemoveRange(borrow.UserItem.EquippedItems);
        db.ClanArmoryBorrows.Remove(borrow);

        return Result.NoErrors;
    }
}
