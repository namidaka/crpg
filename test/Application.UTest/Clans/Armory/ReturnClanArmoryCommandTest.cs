using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Clans.Commands.Armory;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans.Armory;
public class ReturnClanArmoryCommandTest : ClanArmoryTest
{
    [Test]
    public async Task ShouldReturn()
    {
        await AddItems(ArrangeDb, "user0");
        await BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Name == "user1");

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = user.ArmoryBorrows.First().UserItemId,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.ArmoryBorrows)
            .Include(e => e.Items)
            .FirstAsync(e => e.Id == user.Id);

        var clan = AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .Include(e => e.ArmoryBorrows)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryBorrows.Count, Is.EqualTo(user.ArmoryBorrows.Count));
        Assert.That(AssertDb.ClanArmoryBorrows.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldNotReturnWithWrongUser()
    {
        await AddItems(ArrangeDb, "user0");
        await BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryBorrow)
            .FirstAsync(e => e.Name == "user0");

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = user.Items.First(e => e.ClanArmoryBorrow != null).Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Name == "user1");

        var clan = AssertDb.Clans
            .Include(e => e.ArmoryBorrows)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryBorrows.Count, Is.EqualTo(user.ArmoryBorrows.Count));
    }

    [Test]
    public async Task ShouldNotReturnNotExisting()
    {
        await AddItems(ArrangeDb, "user0");
        await BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryItem)
            .FirstAsync(e => e.Name == "user1");

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = user.Items.First(e => e.ClanArmoryItem == null).Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.ArmoryBorrows)
            .Include(e => e.Items)
            .FirstAsync(e => e.Id == user.Id);

        var clan = await AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .Include(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryBorrows.Count, Is.EqualTo(user.ArmoryBorrows.Count));
    }

    [Test]
    public async Task ShouldNotReturnNotBorrowed()
    {
        await AddItems(ArrangeDb, "user0");
        await AddItems(ArrangeDb, "user1");
        await BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryItem)
            .FirstAsync(e => e.Name == "user1");

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = user.Items.First(e => e.ClanArmoryItem != null).Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Id == user.Id);

        var clan = AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .Include(e => e.ArmoryBorrows)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryBorrows.Count, Is.EqualTo(user.ArmoryBorrows.Count));
    }
}
