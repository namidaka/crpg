using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Clans.Commands;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Clans;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans;
public class ArmoryRemoveCommandTest : ArmoryTest
{
    [Test]
    public async Task BasicOp()
    {
        await AddItems("user0");

        var user = await ActDb.Users
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .Include(e => e.ClanMembership).
            FirstAsync(e => e.Name == "user0");

        var handler = new ArmoryRemoveCommand.Handler(ActDb);
        var result = await handler.Handle(new ArmoryRemoveCommand
        {
            UserItemId = user.Items.First(e => e.ArmoryItem != null).Id,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .FirstAsync(e => e.Id == user.Id);

        var clan = AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryItems.Count, Is.EqualTo(0));
        Assert.That(user.Items.Any(e => e.ArmoryItem != null), Is.False);
    }

    [Test]
    public async Task WrongUser()
    {
        await AddItems("user0");

        var user = await ActDb.Users
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var clan = ActDb.Clans
            .Include(e => e.ArmoryItems)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        int expectedCount = clan.ArmoryItems.Count();

        var handler = new ArmoryRemoveCommand.Handler(ActDb);
        var result = await handler.Handle(new ArmoryRemoveCommand
        {
            UserItemId = clan.ArmoryItems.First().UserItemId,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        clan = AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryItems.Count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task NotInArmoryUserItem()
    {
        await AddItems("user0");

        var user = await ActDb.Users
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user0");

        var handler = new ArmoryRemoveCommand.Handler(ActDb);
        var result = await handler.Handle(new ArmoryRemoveCommand
        {
            UserItemId = user.Items.First(e => e.ArmoryItem == null).Id,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        var clan = AssertDb.Clans
            .Include(e => e.ArmoryItems)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        Assert.That(clan.ArmoryItems.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task BusyUserItem()
    {
        await AddItems("user0");
        await BorrowItems("user1");

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem!)
                    .ThenInclude(e => e.Borrow)
            .FirstAsync(e => e.Name == "user0");

        var handler = new ArmoryRemoveCommand.Handler(ActDb);
        var result = await handler.Handle(new ArmoryRemoveCommand
        {
            UserItemId = user.Items.First(e => e.ArmoryItem?.Borrow != null).Id,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ArmoryBorrows)
            .Include(e => e.Items)
            .FirstAsync(e => e.Name == "user1");

        Assert.That(user.ArmoryBorrows.Count, Is.EqualTo(0));
    }
}
