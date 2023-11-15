using Crpg.Application.Clans.Commands.Armory;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans.Armory;
public class BorrowClanArmoryCommandTest : TestBase
{
    private IClanService ClanService { get; } = new ClanService();
    private IActivityLogService ActivityService { get; } = new ActivityLogService();

    [Test]
    public async Task ShouldBorrow()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var clan = await ActDb.Clans
            .Include(e => e.Members).ThenInclude(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var item = clan.Members.First(e => e.ArmoryItems.Count > 0).ArmoryItems.First();

        var handler = new BorrowClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new BorrowClanArmoryCommand
        {
            UserItemId = item.UserItemId,
            UserId = user.Id,
            ClanId = clan.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ClanMembership!.ArmoryBorrows.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldNotBorrowItemInUse()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ClanArmoryTestHelper.BorrowItems(ArrangeDb, "user1");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user2");

        var clan = await ActDb.Clans
            .Include(e => e.Members).ThenInclude(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var item = clan.Members.First(e => e.ArmoryItems.Count > 0).ArmoryItems.First();

        var handler = new BorrowClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new BorrowClanArmoryCommand
        {
            UserItemId = item.UserItemId,
            UserId = user.Id,
            ClanId = clan.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ClanMembership!.ArmoryBorrows.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldNotBorrowWithWrongClan()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var clan = await ActDb.Clans
            .Include(e => e.Members).ThenInclude(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var item = clan.Members.First(e => e.ArmoryItems.Count > 0).ArmoryItems.First();

        var handler = new BorrowClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new BorrowClanArmoryCommand
        {
            UserItemId = item.UserItemId,
            UserId = user.Id,
            ClanId = clan.Id + 1,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
             .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
             .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ClanMembership!.ArmoryBorrows.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldNotBorrowExistingItem()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ArrangeDb.Users
            .Include(e => e.Items)
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var clan = await ArrangeDb.Clans
            .Include(e => e.Members).ThenInclude(e => e.ArmoryItems).ThenInclude(e => e.UserItem)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        var item = clan.Members.First(e => e.ArmoryItems.Count > 0).ArmoryItems.First().UserItem!;
        user.Items.Add(new UserItem { ItemId = item.ItemId });
        await ArrangeDb.SaveChangesAsync();

        user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Id == user.Id);

        var handler = new BorrowClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new BorrowClanArmoryCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
            ClanId = clan.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
             .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
             .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.ClanMembership!.ArmoryBorrows.Count, Is.EqualTo(0));
    }
}
