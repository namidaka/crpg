using Crpg.Application.Clans.Commands.Armory;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans.Armory;
public class AddCanArmoryCommandTest : TestBase
{
    private IClanService ClanService { get; } = new ClanService();
    private IActivityLogService ActivityService { get; } = new ActivityLogService();

    [Test]
    public async Task ShouldAdd()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);

        var user = await ActDb.Users
            .Include(u => u.Items)
            .Include(u => u.ClanMembership)
            .FirstAsync();

        var item = user.Items.First();
        var handler = new AddItemToClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new AddItemToClanArmoryCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(u => u.Items).ThenInclude(ui => ui.ClanArmoryItem)
            .FirstAsync(u => u.Id == user.Id);

        Assert.That(user.Items.Count(ui => ui.ClanArmoryItem != null), Is.EqualTo(1));
        Assert.That(AssertDb.ClanArmoryItems.Count(), Is.EqualTo(1));

        var view = result.Data!;
        Assert.That(view.UserItem, Is.Not.Null);
        Assert.That(view.BorrowedItem, Is.Null);
    }

    [Test]
    public async Task ShouldNotAddTwice()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ArrangeDb.SaveChangesAsync();

        var user = await ActDb.Users
            .Include(u => u.Items).ThenInclude(ui => ui.ClanArmoryItem)
            .Include(u => u.ClanMembership)
            .FirstAsync(u => u.Name == "user0");

        var item = user.Items.First(ui => ui.ClanArmoryItem != null);

        var handler = new AddItemToClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new AddItemToClanArmoryCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.Errors!.First().Code, Is.Not.EqualTo(ErrorCode.InternalError));

        user = await AssertDb.Users
            .Include(u => u.Items).ThenInclude(ui => ui.ClanArmoryItem)
            .FirstAsync(u => u.Id == user.Id);

        Assert.That(user.Items.Count(ui => ui.ClanArmoryItem != null), Is.EqualTo(1));
        Assert.That(AssertDb.ClanArmoryItems.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldNotAddWithWrongUser()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        var user0 = await ActDb.Users
            .Include(u => u.Items)
            .FirstAsync();

        var user1 = await ActDb.Users
            .Include(u => u.Items)
            .Include(u => u.ClanMembership)
            .FirstAsync(u => u.Id != user0.Id);

        var item = user0.Items.First();

        var handler = new AddItemToClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new AddItemToClanArmoryCommand
        {
            UserItemId = item.Id,
            UserId = user1.Id,
            ClanId = user1.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        var user = await AssertDb.Users
            .Include(u => u.Items).ThenInclude(ui => ui.ClanArmoryItem)
            .FirstAsync(u => u.Id == user1.Id);

        Assert.That(user.Items.Count(ui => ui.ClanArmoryItem != null), Is.EqualTo(0));
        Assert.That(AssertDb.ClanArmoryItems.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldNotAddWithWrongClan()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        var user = await ActDb.Users
            .Include(u => u.Items)
            .Include(u => u.ClanMembership)
            .FirstAsync();

        var item = user.Items.First();

        var handler = new AddItemToClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new AddItemToClanArmoryCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId + 1,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(u => u.Items).ThenInclude(ui => ui.ClanArmoryItem)
            .FirstAsync(u => u.Id == user.Id);

        Assert.That(user.Items.Count(ui => ui.ClanArmoryItem != null), Is.EqualTo(0));
        Assert.That(AssertDb.ClanArmoryItems.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldNotAddEquippedItem()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        var user = await ArrangeDb.Users
            .Include(u => u.Items)
            .Include(u => u.Characters)
            .FirstAsync();

        var item = user.Items.First();
        var equippedItem = new EquippedItem { Character = user.Characters.First(), UserItem = item };
        await ArrangeDb.EquippedItems.AddAsync(equippedItem);
        await ArrangeDb.SaveChangesAsync();

        user = await ActDb.Users
            .Include(u => u.Items)
            .Include(u => u.ClanMembership)
            .FirstAsync(u => u.Name == user.Name);

        var handler = new AddItemToClanArmoryCommand.Handler(ActDb, Mapper, ActivityService, ClanService);
        var result = await handler.Handle(new AddItemToClanArmoryCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
            ClanId = user.ClanMembership!.ClanId,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(u => u.Items).ThenInclude(ui => ui.ClanArmoryItem)
            .FirstAsync(u => u.Id == user.Id);

        Assert.That(user.Items.Count(ui => ui.ClanArmoryItem != null), Is.EqualTo(0));
        Assert.That(AssertDb.ClanArmoryItems.Count(), Is.EqualTo(0));
    }
}
