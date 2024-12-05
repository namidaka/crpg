using Crpg.Application.Common.Results;
using Crpg.Application.Items.Commands;
using Crpg.Domain.Entities.Characters;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Items;

public class RefundItemCommandTest : TestBase
{
    [Test]
    public async Task ShouldReturnErrorIfItemIsNotFound()
    {
        User user = new();
        ArrangeDb.Users.Add(user);
        await ArrangeDb.SaveChangesAsync();

        var result = await new RefundItemCommand.Handler(ActDb).Handle(new RefundItemCommand
        {
            ItemId = "a",
            UserId = user.Id,
        }, CancellationToken.None);
        Assert.That(result.Errors![0].Code, Is.EqualTo(ErrorCode.ItemNotFound));
    }

    [Test]
    public async Task ShouldRefundItem()
    {
        Item item = new() { Id = "a", Enabled = true, Price = 100, Rank = 1 };
        ArrangeDb.Items.Add(item);
        User user = new();
        ArrangeDb.Users.Add(user);
        UserItem userItem = new() { User = user, ItemId = item.Id };
        ArrangeDb.UserItems.Add(userItem);
        await ArrangeDb.SaveChangesAsync();

        var result = await new RefundItemCommand.Handler(ActDb).Handle(new RefundItemCommand
        {
            ItemId = item.Id,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);
        user = AssertDb.Users.First(u => u.Id == user.Id);
        Assert.That(user.Gold, Is.EqualTo(100));
        Assert.That(user.HeirloomPoints, Is.EqualTo(1));
    }
}
