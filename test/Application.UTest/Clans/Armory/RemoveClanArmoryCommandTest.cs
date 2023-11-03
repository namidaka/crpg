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
public class RemoveClanArmoryCommandTest : TestBase
{
    private IClanService ClanService { get; } = new ClanService();
    private IActivityLogService ActivityService { get; } = new ActivityLogService();

    [Test]
    public async Task ShouldRemove()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryItem)
            .Include(e => e.ClanMembership).
            FirstAsync(e => e.Name == "user0");

        var handler = new RemoveClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new RemoveClanArmoryCommand
        {
            UserItemId = user.Items.First(e => e.ClanArmoryItem != null).Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.Id);
        Assert.That(user.ClanMembership!.ArmoryItems.Count, Is.EqualTo(0));

        Assert.That(AssertDb.ClanArmoryItems.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldNotRemoveWithWrongUser()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership!)
            .FirstAsync(e => e.Name == "user1");

        var clan = ActDb.Clans
            .Include(e => e.Members).ThenInclude(e => e.ArmoryItems)
            .First(e => e.Id == user.ClanMembership!.ClanId);

        var items = clan.Members.SelectMany(e => e.ArmoryItems);
        int expectedCount = items.Count();

        var handler = new RemoveClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new RemoveClanArmoryCommand
        {
            UserItemId = items.First().UserItemId,
            UserId = user.Id,
            ClanId = clan.Id,
        }, CancellationToken.None);
        Assert.That(result.Errors, Is.Not.Empty);

        Assert.That(AssertDb.ClanArmoryItems.Count(), Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task ShouldNotRemoveNotExisting()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryItem)
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user0");

        var handler = new RemoveClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new RemoveClanArmoryCommand
        {
            UserItemId = user.Items.First(e => e.ClanArmoryItem == null).Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        Assert.That(AssertDb.ClanArmoryItems.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldRemoveBorrowed()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ClanArmoryTestHelper.BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryItems).ThenInclude(e => e.Borrow)
            .FirstAsync(e => e.Name == "user0");

        var item = user.ClanMembership!.ArmoryItems.First(e => e.Borrow != null);

        var handler = new RemoveClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new RemoveClanArmoryCommand
        {
            UserItemId = item.UserItemId,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
            .Where(e => e.Name == "user1")
            .FirstAsync();
        Assert.That(user.ClanMembership!.ArmoryBorrows.Count, Is.EqualTo(0));

        Assert.That(AssertDb.ClanArmoryBorrows.Count(), Is.EqualTo(0));
    }
}
