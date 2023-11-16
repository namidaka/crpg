using Crpg.Application.Clans.Commands.Armory;
using Crpg.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans.Armory;
public class ReturnClanArmoryCommandTest : TestBase
{
    private IClanService ClanService { get; } = new ClanService();
    private IActivityLogService ActivityService { get; } = new ActivityLogService();

    [Test]
    public async Task ShouldReturn()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ClanArmoryTestHelper.BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrowedItems)
            .FirstAsync(e => e.Name == "user1");

        var item = user.ClanMembership!.ArmoryBorrowedItems.First();

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, ActivityService, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = item.UserItemId,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrowedItems)
            .Include(e => e.Items)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ClanMembership!.ArmoryBorrowedItems.Count, Is.EqualTo(0));
        Assert.That(AssertDb.ClanArmoryBorrowedItems.Count(), Is.EqualTo(0));
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
            .Include(e => e.Items).ThenInclude(e => e.ClanArmoryBorrowedItem)
            .FirstAsync(e => e.Name == "user0");

        var item = user.Items.First(e => e.ClanArmoryBorrowedItem != null);

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, ActivityService, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrowedItems)
            .FirstAsync(e => e.Name == "user1");

        Assert.That(AssertDb.ClanArmoryBorrowedItems.Count(), Is.EqualTo(1));
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

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, ActivityService, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = user.Items.First(e => e.ClanArmoryItem == null).Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrowedItems)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ClanMembership!.ArmoryBorrowedItems.Count, Is.EqualTo(1));
        Assert.That(AssertDb.ClanArmoryBorrowedItems.Count(), Is.EqualTo(1));
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

        var handler = new ReturnClanArmoryCommand.Handler(ActDb, ActivityService, ClanService);
        var result = await handler.Handle(new ReturnClanArmoryCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrowedItems)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ClanMembership!.ArmoryBorrowedItems.Count, Is.EqualTo(1));
        Assert.That(AssertDb.ClanArmoryBorrowedItems.Count(), Is.EqualTo(1));
    }
}
