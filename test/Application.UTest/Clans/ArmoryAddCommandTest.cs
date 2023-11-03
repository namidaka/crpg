using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Application.Clans.Commands;
using Crpg.Application.Common.Services;
using Crpg.Application.Items.Commands;
using Crpg.Domain.Entities.Clans;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Clans;
public class ArmoryAddCommandTest : ArmoryTest
{
    [Test]
    public async Task BasicOp()
    {
        var user = await ActDb.Users
            .Include(e => e.Items)
            .Include(e => e.ClanMembership).
            FirstAsync();

        var item = user.Items.First();

        var handler = new ArmoryAddCommand.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryAddCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);

        user = await AssertDb.Users
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .FirstAsync(e => e.Id == user.Id);

        var view = result.Data!;

        Assert.That(user.Items.Count(e => e.ArmoryItem != null), Is.EqualTo(1));
        Assert.That(AssertDb.ArmoryItems.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task BrokenItem()
    {
        var user = await ArrangeDb.Users
            .Include(e => e.Items)
            .FirstAsync();

        var item = user.Items.First();
        item.IsBroken = true;

        await ArrangeDb.SaveChangesAsync();

        user = await ActDb.Users
            .Include(e => e.Items)
            .FirstAsync(e => e.Name == user.Name);

        var handler = new ArmoryAddCommand.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryAddCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.Items.Count(e => e.ArmoryItem != null), Is.EqualTo(0));
        Assert.That(AssertDb.ArmoryItems.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task WrongUser()
    {
        var user0 = await ActDb.Users
            .Include(e => e.Items)
            .Include(e => e.ClanMembership)
            .FirstAsync();

        var user1 = await ActDb.Users
            .Include(e => e.Items)
            .FirstAsync(e => e.Id != user0.Id);

        var item = user0.Items.First();

        var handler = new ArmoryAddCommand.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryAddCommand
        {
            UserItemId = item.Id,
            UserId = user1.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        var user = await AssertDb.Users
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .FirstAsync(e => e.Id == user1.Id);

        Assert.That(user.Items.Count(e => e.ArmoryItem != null), Is.EqualTo(0));
        Assert.That(AssertDb.ArmoryItems.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task NoClan()
    {
        var user = await ArrangeDb.Users
            .Include(e => e.Items)
            .Include(e => e.ClanMembership)
            .FirstAsync();

        user.ClanMembership = null;
        await ArrangeDb.SaveChangesAsync();

        user = await ActDb.Users
            .Include(e => e.Items)
            .FirstAsync(e => e.Name == user.Name);

        var item = user.Items.First();

        var handler = new ArmoryAddCommand.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryAddCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.Items.Count(e => e.ArmoryItem != null), Is.EqualTo(0));
        Assert.That(AssertDb.ArmoryItems.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task EquippedItem()
    {
        var user = await ArrangeDb.Users
            .Include(e => e.Items)
            .Include(e => e.Characters)
            .FirstAsync();

        var item = user.Items.First();
        var equippedItem = new EquippedItem { Character = user.Characters.First(), UserItem = item };
        await ArrangeDb.EquippedItems.AddAsync(equippedItem);
        await ArrangeDb.SaveChangesAsync();

        user = await ActDb.Users
            .Include(e => e.Items)
            .FirstAsync(e => e.Name == user.Name);

        var handler = new ArmoryAddCommand.Handler(ActDb, Mapper);
        var result = await handler.Handle(new ArmoryAddCommand
        {
            UserItemId = item.Id,
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Not.Empty);

        user = await AssertDb.Users
            .Include(e => e.Items)
                .ThenInclude(e => e.ArmoryItem)
            .FirstAsync(e => e.Id == user.Id);

        Assert.That(user.Items.Count(e => e.ArmoryItem != null), Is.EqualTo(0));
        Assert.That(AssertDb.ArmoryItems.Count(), Is.EqualTo(0));
    }
}
