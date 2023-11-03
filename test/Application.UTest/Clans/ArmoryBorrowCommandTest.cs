using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Clans.Commands;
using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans;
public class ArmoryBorrowCommandTest : ArmoryTest
{
    [Test]
    public async Task BasicOp()
    {
        await AddItems("user0");

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var clan = await ActDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var handler = new ArmoryBorrowCommand.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryBorrowCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.ArmoryBorrows)
            .Include(e => e.Items)
            .FirstAsync(e => e.Id == user.Id);

        clan = AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .Include(e => e.ArmoryBorrows)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryBorrows.Count, Is.EqualTo(1));
        Assert.That(clan.ArmoryBorrows.Count, Is.EqualTo(user.ArmoryBorrows.Count));
    }

    [Test]
    public async Task BusyItem()
    {
        await AddItems("user0");
        await BorrowItems("user1");

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user2");

        var clan = await ActDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var handler = new ArmoryBorrowCommand.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryBorrowCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Id == user.Id);

        clan = AssertDb.Clans
            .Include(e => e.ArmoryBorrows)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryBorrows.Count, Is.EqualTo(1));
        Assert.That(user.ArmoryBorrows, Is.Empty);
    }

    [Test]
    public async Task NoClan()
    {
        await AddItems("user0");

        var user1 = await ArrangeDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        user1.ClanMembership = null;
        await ArrangeDb.SaveChangesAsync();

        var user0 = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user0");

        var clan = await ActDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user0.ClanMembership!.ClanId);

        var handler = new ArmoryBorrowCommand.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryBorrowCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user1.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        var user = await AssertDb.Users
             .Include(e => e.ArmoryBorrows)
             .FirstAsync(e => e.Id == user1.Id);

        Assert.That(user.ArmoryBorrows, Is.Empty);
    }

    [Test]
    public async Task ExistingItem()
    {
        await AddItems("user0");

        var user = await ArrangeDb.Users
            .Include(e => e.Items)
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var clan = await ArrangeDb.Clans
            .Include(e => e.ArmoryItems)
                .ThenInclude(e => e.UserItem)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        user.Items.Add(new UserItem { ItemId = clan.ArmoryItems.First().UserItem!.ItemId });
        await ArrangeDb.SaveChangesAsync();

        user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Id == user.Id);

        clan = await ActDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var handler = new ArmoryBorrowCommand.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryBorrowCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
             .Include(e => e.ArmoryBorrows)
             .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ArmoryBorrows, Is.Empty);
    }

    [Test]
    public async Task ExistingUserItem()
    {
        await AddItems("user0");

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user0");

        var clan = await ActDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var handler = new ArmoryBorrowCommand.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryBorrowCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
             .Include(e => e.ArmoryBorrows)
             .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ArmoryBorrows, Is.Empty);
    }
}
