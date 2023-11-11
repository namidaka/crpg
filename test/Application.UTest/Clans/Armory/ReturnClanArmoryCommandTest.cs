using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Clans.Commands.Armory;
using Crpg.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans.Armory;
public class ReturnClanArmoryCommandTest : TestBase
{
    private IClanService ClanService { get; } = new ClanService();

    [Test]
    public async Task ShouldReturn()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ClanArmoryTestHelper.BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Name == "user1");

        var item = user.ClanMembership!.ArmoryBorrows.First();

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = item.UserItemId,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
            .Include(e => e.Items)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ClanMembership!.ArmoryBorrows.Count, Is.EqualTo(0));
        Assert.That(AssertDb.ClanArmoryBorrows.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldNotReturnWithWrongUser()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ClanArmoryTestHelper.BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryBorrow)
            .FirstAsync(e => e.Name == "user0");

        var item = user.Items.First(e => e.ClanArmoryBorrow != null);

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Name == "user1");

        Assert.That(AssertDb.ClanArmoryBorrows.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldNotReturnNotExisting()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ClanArmoryTestHelper.BorrowItems(ArrangeDb, "user1");
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
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ClanMembership!.ArmoryBorrows.Count, Is.EqualTo(1));
        Assert.That(AssertDb.ClanArmoryBorrows.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldNotReturnNotBorrowed()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user1");
        await ClanArmoryTestHelper.BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryItem)
            .FirstAsync(e => e.Name == "user1");

        var item = user.Items.First(e => e.ClanArmoryItem != null);

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, Mapper, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ClanMembership!.ArmoryBorrows.Count, Is.EqualTo(1));
        Assert.That(AssertDb.ClanArmoryBorrows.Count(), Is.EqualTo(1));
    }
}
