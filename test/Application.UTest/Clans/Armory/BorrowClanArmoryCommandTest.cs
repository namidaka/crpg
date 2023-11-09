using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Clans.Commands.Armory;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans.Armory;
public class BorrowClanArmoryCommandTest : ClanArmoryTest
{
    [Test]
    public async Task ShouldBorrow()
    {
        await AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var clan = await ActDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var handler = new BorrowClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new BorrowClanArmoryCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user.Id,
            ClanId = clan.Id,
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
    public async Task ShouldNotBorrowBusyItem()
    {
        await AddItems(ArrangeDb, "user0");
        await BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user2");

        var clan = await ActDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var handler = new BorrowClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new BorrowClanArmoryCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user.Id,
            ClanId = clan.Id,
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
    public async Task ShouldNotBorrowWithWrongClan()
    {
        await AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var clan = await ActDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var handler = new BorrowClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new BorrowClanArmoryCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user.Id,
            ClanId = clan.Id + 1,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
             .Include(e => e.ArmoryBorrows)
             .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ArmoryBorrows, Is.Empty);
    }

    [Test]
    public async Task ShouldNotBorrowExistingItem()
    {
        await AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ArrangeDb.Users
            .Include(e => e.Items)
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var clan = await ArrangeDb.Clans
            .Include(e => e.ArmoryItems).ThenInclude(e => e.UserItem)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        user.Items.Add(new UserItem { ItemId = clan.ArmoryItems.First().UserItem!.ItemId });
        await ArrangeDb.SaveChangesAsync();

        user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Id == user.Id);

        clan = await ActDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var handler = new BorrowClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new BorrowClanArmoryCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user.Id,
            ClanId = clan.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
             .Include(e => e.ArmoryBorrows)
             .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ArmoryBorrows, Is.Empty);
    }
}
