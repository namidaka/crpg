using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Characters;
using Crpg.Domain.Entities.Clans;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans.Armory;
public class ClanArmoryTest : TestBase
{
    internal IClanService ClanService { get; init; } = new ClanService();

    [SetUp]
    public async Task CommonSetup()
    {
        int nusers = 4;
        int itemsPerUser = 4;

        var items = Enumerable.Range(0, nusers * itemsPerUser).Select(idx => new Item { Id = $"{idx}", Name = $"item{idx}" }).ToList();
        var clan = new Clan { };

        var users = Enumerable.Range(0, nusers).Select(idx => new User
        {
            Name = $"user{idx}",
            ClanMembership = new() { Clan = clan },
            Characters = Enumerable.Range(0, 2).Select(i => new Character()).ToList(),
            Items = items.GetRange(idx * itemsPerUser, itemsPerUser).Select(e => new UserItem { Item = e }).ToList(),
        });

        ArrangeDb.Users.AddRange(users);
        ArrangeDb.Items.AddRange(items);
        ArrangeDb.Clans.AddRange(clan);
        await ArrangeDb.SaveChangesAsync();

        Assert.That(ArrangeDb.Users.Count(), Is.EqualTo(users.Count()));
        Assert.That(ArrangeDb.Items.Count(), Is.EqualTo(items.Count()));
        Assert.That(ArrangeDb.Clans.Count(), Is.EqualTo(1));
        Assert.That(ArrangeDb.ClanMembers.Count(), Is.EqualTo(users.Count()));
        Assert.That(ArrangeDb.UserItems.Count(), Is.EqualTo(users.Count() * itemsPerUser));
    }

    public async Task AddItems(ICrpgDbContext db, string userName, int count = 1)
    {
        var user = await db.Users
            .Include(e => e.Items)
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == userName);

        var clan = await db.Clans
            .Include(e => e.ArmoryItems)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        foreach (var item in user.Items.Take(count))
        {
            clan.ArmoryItems.Add(new ClanArmoryItem { UserItem = item });
        }
    }

    public async Task BorrowItems(ICrpgDbContext db, string userName, int count = 1)
    {
        var user = await db.Users
            .Include(e => e.Items)
            .Include(e => e.ClanMembership)
            .FirstAsync(e => e.Name == userName);

        var clan = await db.Clans
            .Include(e => e.ArmoryItems)
            .Include(e => e.ArmoryBorrows)
            .FirstAsync(e => e.Id == user.ClanMembership!.ClanId);

        foreach (var item in clan.ArmoryItems.Take(count))
        {
            clan.ArmoryBorrows.Add(new ClanArmoryBorrow { ArmoryItem = item, User = user });
        }
    }
}
