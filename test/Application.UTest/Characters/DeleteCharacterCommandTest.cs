using Crpg.Application.Characters.Commands;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Characters;
using Crpg.Sdk.Abstractions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Characters;

public class DeleteCharacterCommandTest : TestBase
{
    private static readonly Mock<IActivityLogService> ActivityLogService = new() { DefaultValue = DefaultValue.Mock };
    private static readonly IDateTime DateTime = Mock.Of<IDateTime>();

    [Test]
    public async Task WhenCharacterExists()
    {
        var character = new Character
        {
            Name = "sword",
            UserId = 1,
        };
        ArrangeDb.Characters.Add(character);
        await ArrangeDb.SaveChangesAsync();

        DeleteCharacterCommand.Handler handler = new(ActDb, DateTime, ActivityLogService.Object);
        await handler.Handle(new DeleteCharacterCommand
        {
            CharacterId = character.Id,
            UserId = character.UserId,
        }, CancellationToken.None);

        var characterDb = await AssertDb.Characters
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == character.Id);
        Assert.That(characterDb, Is.Not.Null);
        Assert.That(characterDb!.DeletedAt, Is.Not.Null);
    }

    [Test]
    public async Task WhenCharacterExistsButNotOwnedByUser()
    {
        var character = new Character
        {
            Name = "sword",
            UserId = 2,
        };
        ArrangeDb.Characters.Add(character);
        await ArrangeDb.SaveChangesAsync();

        DeleteCharacterCommand.Handler handler = new(ActDb, DateTime, ActivityLogService.Object);
        var result = await handler.Handle(new DeleteCharacterCommand
        {
            CharacterId = character.Id,
            UserId = 1,
        }, CancellationToken.None);

        Assert.That(result.Errors![0].Code, Is.EqualTo(ErrorCode.CharacterNotFound));
    }

    [Test]
    public async Task WhenCharacterDoesntExist()
    {
        DeleteCharacterCommand.Handler handler = new(ActDb, DateTime, ActivityLogService.Object);
        var result = await handler.Handle(new DeleteCharacterCommand { CharacterId = 1 }, CancellationToken.None);
        Assert.That(result.Errors![0].Code, Is.EqualTo(ErrorCode.CharacterNotFound));
    }
}
