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
public class ReturnUnusedClanArmoryItemsCommandTest : TestBase
{
    private IClanService ClanService { get; } = new ClanService();
    private IActivityLogService ActivityService { get; } = new ActivityLogService();

    [Test]
    public async Task ShouldReturn()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0", 2);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user1", 2);
        await ClanArmoryTestHelper.BorrowItems(ArrangeDb, "user2", 2);
        await ClanArmoryTestHelper.BorrowItems(ArrangeDb, "user3", 2);
        await ArrangeDb.Users.ForEachAsync(u => u.UpdatedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10)));
        await ArrangeDb.SaveChangesAsync();

        Assert.That(ActDb.ClanArmoryBorrows.Count(), Is.EqualTo(4));

        var handler = new ReturnUnusedClanArmoryItemsCommand.Handler(ActDb, ClanService);
        var result = await handler.Handle(new ReturnUnusedClanArmoryItemsCommand
        {
            Timeout = TimeSpan.FromDays(3),
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        Assert.That(AssertDb.ClanArmoryBorrows.Count(), Is.EqualTo(0));
    }
}
