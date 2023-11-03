using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Clans.Queries;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans;
public class ArmoryGetListQueryTest : ArmoryTest
{
    [Test]
    public async Task BasicOp()
    {
        int count = 2;
        await AddItems("user0", count);
        await BorrowItems("user1", count);

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == "user1");

        var handler = new ArmoryGetListQuery.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryGetListQuery { UserId = user.Id }, CancellationToken.None);

        var items = result.Data!;
        var item = items.First();

        Assert.That(result.Errors, Is.Null);
        Assert.That(items.Count, Is.EqualTo(count));
        Assert.That(item.UserItem, Is.Not.Null);
        Assert.That(item.UserItem!.Item, Is.Not.Null);
        Assert.That(item.Borrow, Is.Not.Null);
    }
}
