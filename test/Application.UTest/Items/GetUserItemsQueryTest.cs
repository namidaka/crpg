using Crpg.Application.Items.Queries;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;
using NUnit.Framework;

namespace Crpg.Application.UTest.Items;

public class GetUserItemsQueryTest : TestBase
{
    [Test]
    public async Task Basic()
    {
        var user = ArrangeDb.Users.Add(new User
        {
            Items =
            {
                new() { Item = new Item { Id = "1", Enabled = true } },
                new() { Item = new Item { Id = "2", Enabled = true } },
                new() { Item = new Item { Id = "3", Enabled = false } },
            },
        });
        await ArrangeDb.SaveChangesAsync();

        var result = await new GetUserItemsQuery.Handler(ActDb, Mapper).Handle(
            new GetUserItemsQuery { UserId = user.Entity.Id }, CancellationToken.None);

        Assert.That(result.Data!.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task PersonalItems()
    {
        var user = ArrangeDb.Users.Add(new User
        {
            Items =
            {
                new() { Item = new Item { Id = "1", Enabled = true } },
                new() { Item = new Item { Id = "2", Enabled = true } },
                new() { Item = new Item { Id = "3", Enabled = false, }, PersonalItem = new() },
            },
        });
        await ArrangeDb.SaveChangesAsync();

        var result = await new GetUserItemsQuery.Handler(ActDb, Mapper).Handle(
            new GetUserItemsQuery { UserId = user.Entity.Id }, CancellationToken.None);

        Assert.That(result.Data!.Count, Is.EqualTo(3));
    }
}
