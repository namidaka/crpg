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

public class ClanArmoryCasesTest : TestBase
{
    private IClanService ClanService { get; } = new ClanService();

    [Test]
    public async Task ShouldCascadeOnClanLeave()
    {
        await ClanArmoryTestHelper.CommonSetUp(ArrangeDb);
        await ClanArmoryTestHelper.AddItems(ArrangeDb, "user0");
        await ClanArmoryTestHelper.BorrowItems(ArrangeDb, "user1");

        var user = await ActDb.Users
            .Include(e => e.ClanMembership)
            .Where(e => e.Name == "user0")
            .FirstAsync();

        var result = await ClanService.LeaveClan(ActDb, user.ClanMembership!, CancellationToken.None);
        Assert.That(result.Errors, Is.Null.Or.Empty);
        await ActDb.SaveChangesAsync();

        Assert.That(AssertDb.ClanArmoryBorrows.Count(), Is.EqualTo(0));
        Assert.That(AssertDb.ClanArmoryItems.Count(), Is.EqualTo(0));

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryItems)
            .Where(e => e.Name == "user0")
            .FirstAsync();

        Assert.That(user.ClanMembership, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryItems)
            .Include(e => e.ClanMembership!).ThenInclude(e => e.ArmoryBorrows)
            .Include(e => e.Items)
            .Where(e => e.Name == "user1")
            .FirstAsync();

        Assert.That(user.ClanMembership, Is.Not.Null);
        Assert.That(user.ClanMembership!.ArmoryItems.Count, Is.EqualTo(0));
        Assert.That(user.ClanMembership!.ArmoryBorrows.Count, Is.EqualTo(0));
    }
}
