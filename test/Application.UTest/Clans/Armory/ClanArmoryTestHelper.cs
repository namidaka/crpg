using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Characters;
using Crpg.Domain.Entities.Clans;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;
using Crpg.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans.Armory;
public static class ClanArmoryTestHelper
{
    private static IClanService ClanService { get; } = new ClanService();

    public static async Task CommonSetUp(ICrpgDbContext db, int nusers = 4, int itemsPerUser = 4)
    {
        var items = Enumerable.Range(0, nusers * itemsPerUser).Select(idx => new Item { Id = $"{idx}", Name = $"item{idx}" }).ToList();
        var clan = new Clan { };

        var users = Enumerable.Range(0, nusers).Select(idx => new User
        {
            Name = $"user{idx}",
            ClanMembership = new() { Clan = clan },
            Characters = Enumerable.Range(0, 2).Select(i => new Character()).ToList(),
            Items = items.GetRange(idx * itemsPerUser, itemsPerUser).Select(e => new UserItem { Item = e }).ToList(),
        });

        db.Users.AddRange(users);
        db.Items.AddRange(items);
        db.Clans.AddRange(clan);
        await db.SaveChangesAsync();

        Assert.That(db.Users.Count(), Is.EqualTo(users.Count()));
        Assert.That(db.Items.Count(), Is.EqualTo(items.Count()));
        Assert.That(db.Clans.Count(), Is.EqualTo(1));
        Assert.That(db.ClanMembers.Count(), Is.EqualTo(users.Count()));
        Assert.That(db.UserItems.Count(), Is.EqualTo(users.Count() * itemsPerUser));
    }

    public static async Task<IList<ClanArmoryItem>> AddItems(ICrpgDbContext db, string userName, int count = 1)
    {
        var user = await db.Users
            .Include(e => e.Items)
            .Include(e => e.ClanMembership)
            .Where(e => e.Name == userName)
            .FirstAsync();

        var clan = await db.Clans
            .Where(e => e.Id == user.ClanMembership!.ClanId)
            .FirstAsync();

        var list = new List<ClanArmoryItem>();
        foreach (var item in user.Items.Take(count))
        {
            var result = await ClanService.AddArmoryItem(db, clan, user, item.Id);
            Assert.That(result.Errors, Is.Null.Or.Empty);
            await db.SaveChangesAsync();

            list.Add(result.Data!);
        }

        return list;
    }

    public static async Task<IList<ClanArmoryBorrow>> BorrowItems(ICrpgDbContext db, string userName, int count = 1)
    {
        var user = await db.Users
            .Include(e => e.Items)
            .Include(e => e.ClanMembership)
            .Where(e => e.Name == userName)
            .FirstAsync();

        var clan = await db.Clans
            .Include(e => e.Members).ThenInclude(e => e.ArmoryItems)
            .Where(e => e.Id == user.ClanMembership!.ClanId)
            .FirstAsync();

        var items = clan.Members.SelectMany(e => e.ArmoryItems).Take(count);
        var list = new List<ClanArmoryBorrow>();
        foreach (var item in items)
        {
            var result = await ClanService.BorrowArmoryItem(db, clan, user, item.UserItemId);
            Assert.That(result.Errors, Is.Null.Or.Empty);
            await db.SaveChangesAsync();

            list.Add(result.Data!);
        }

        return list;
    }
}
