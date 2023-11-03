using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Clans.Commands.Armory;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Clans;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans.Armory;
public class RemoveClanArmoryCommandTest : ClanArmoryTest
{
    [Test]
    public async Task ShouldRemove()
    {
        await AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryItem)
            .Include(e => e.ClanMembership).
            FirstAsync(e => e.Name == "user0");

        var handler = new RemoveClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new RemoveClanArmoryCommand
        {
            UserItemId = user.Items.First(e => e.ClanArmoryItem != null).Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryItem)
            .FirstAsync(e => e.Id == user.Id);

        var clan = AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryItems.Count, Is.EqualTo(0));
        Assert.That(user.Items.Any(e => e.ClanArmoryItem != null), Is.False);
    }

    [Test]
    public async Task ShouldNotRemoveWithWrongUser()
    {
        await AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryItem)
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var clan = ActDb.Clans
            .Include(e => e.ArmoryItems)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        int expectedCount = clan.ArmoryItems.Count();

        var handler = new RemoveClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new RemoveClanArmoryCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user.Id,
            ClanId = clan.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        clan = AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryItems.Count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task ShouldNotRemoveNotExisting()
    {
        await AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryItem)
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user0");

        var handler = new RemoveClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new RemoveClanArmoryCommand
        {
            UserItemId = user.Items.First(e => e.ClanArmoryItem == null).Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        var clan = await AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryItems.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldRemoveBorrowed()
    {
        await AddItems(ArrangeDb, "user0");
        await BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryItem!).ThenInclude(e => e.Borrow)
            .FirstAsync(e => e.Name == "user0");

        var handler = new RemoveClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new RemoveClanArmoryCommand
        {
            UserItemId = user.Items.First(e => e.ClanArmoryItem?.Borrow != null).Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.ArmoryBorrows)
            .Include(e => e.Items)
            .FirstAsync(e => e.Name == "user1");

        var clan = await AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(user.ArmoryBorrows.Count, Is.EqualTo(0));
        Assert.That(clan.ArmoryItems.Count, Is.EqualTo(0));
    }
}
