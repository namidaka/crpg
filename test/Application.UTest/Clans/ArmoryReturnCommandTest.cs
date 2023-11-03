using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Clans.Commands;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans;
public class ArmoryReturnCommandTest : ArmoryTest
{
    [Test]
    public async Task BasicOp()
    {
        await AddItems("user0");
        await BorrowItems("user1");

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Name == "user1");

        var handler = new ArmoryReturnCommand.Handler(ActDb);
        var result = await handler.Handle(new ArmoryReturnCommand
        {
            UserItemId = user.ArmoryBorrows.First().UserItemId,
            UserId = user.Id,
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
        Assert.That(AssertDb.ArmoryBorrows.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task WrongUser()
    {
        await AddItems("user0");
        await BorrowItems("user1");

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryBorrow)
            .FirstAsync(e => e.Name == "user0");

        var handler = new ArmoryReturnCommand.Handler(ActDb);
        var result = await handler.Handle(new ArmoryReturnCommand
        {
            UserItemId = user.Items.First(e => e.ArmoryBorrow != null).Id,
            UserId = user.Id,
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
    public async Task NotInArmory()
    {
        await AddItems("user0");
        await BorrowItems("user1");

        var user = await ActDb.Users
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .FirstAsync(e => e.Name == "user1");

        var handler = new ArmoryReturnCommand.Handler(ActDb);
        var result = await handler.Handle(new ArmoryReturnCommand
        {
            UserItemId = user.Items.First(e => e.ArmoryItem == null).Id,
            UserId = user.Id,
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
    public async Task NotBorrowed()
    {
        await AddItems("user0");
        await AddItems("user1");
        await BorrowItems("user1");

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .FirstAsync(e => e.Name == "user1");

        var handler = new ArmoryReturnCommand.Handler(ActDb);
        var result = await handler.Handle(new ArmoryReturnCommand
        {
            UserItemId = user.Items.First(e => e.ArmoryItem != null).Id,
            UserId = user.Id,
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
