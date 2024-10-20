using Crpg.Application.Captains.Commands;
using Crpg.Application.Captains.Queries;
using Crpg.Domain.Entities.Captains;
using Crpg.Domain.Entities.Characters;
using NUnit.Framework;

namespace Crpg.Application.UTest.Captains;

public class UpdateFormationCharacterCommandTest : TestBase
{
    [Test]
    public async Task ShouldUpdateIfCharacterExists()
    {
        Character character = new()
        {
            Name = "toto",
            UserId = 2,
        };
        ArrangeDb.Characters.Add(character);
        await ArrangeDb.SaveChangesAsync();

        Captain captain = new()
        {
            UserId = character.UserId,
            Formations = new List<CaptainFormation>() { new() { Number = 1, Weight = 33 } },
        };

        ArrangeDb.Captains.Add(captain);
        await ArrangeDb.SaveChangesAsync();

        var result = await new UpdateFormationCharacterCommand.Handler(ActDb, Mapper).Handle(new UpdateFormationCharacterCommand
        {
            UserId = character.UserId,
            CharacterId = character.Id,
            Number = 1,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Not.Null);
    }

    [Test]
    public async Task ShouldReturnErrorIfCaptainDoesntExist()
    {
        var result = await new UpdateFormationCharacterCommand.Handler(ActDb, Mapper).Handle(new UpdateFormationCharacterCommand
        {
            UserId = 1,
            Number = 1,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Null);
        Assert.That(result.Errors!, Is.Not.Empty);
    }

    [Test]
    public async Task ShouldReturnErrorIfCharacterDoesntExist()
    {
        Captain captain = new()
        {
            UserId = 1,
            Formations = new List<CaptainFormation>() { new() { Number = 1, Weight = 33 } },
        };

        ArrangeDb.Captains.Add(captain);
        await ArrangeDb.SaveChangesAsync();

        var result = await new UpdateFormationCharacterCommand.Handler(ActDb, Mapper).Handle(new UpdateFormationCharacterCommand
        {
            UserId = captain.UserId,
            CharacterId = 1,
            Number = 1,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Null);
        Assert.That(result.Errors!, Is.Not.Empty);
    }

    [Test]
    public async Task ShouldAssignNullCharacter()
    {
        Captain captain = new()
        {
            UserId = 1,
            Formations = new List<CaptainFormation>() { new() { Number = 1, Weight = 33 } },
        };

        ArrangeDb.Captains.Add(captain);
        await ArrangeDb.SaveChangesAsync();

        var result = await new UpdateFormationCharacterCommand.Handler(ActDb, Mapper).Handle(new UpdateFormationCharacterCommand
        {
            UserId = captain.UserId,
            CharacterId = null,
            Number = 1,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Not.Null);
    }
}
