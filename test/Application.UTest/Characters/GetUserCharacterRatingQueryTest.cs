using Crpg.Application.Characters.Queries;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Characters;
using Crpg.Domain.Entities.Servers;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Characters;

public class GetUserCharacterRatingQueryTest : TestBase
{
    [Test]
    public async Task ShouldReturnErrorIfCharacterRatingDoesntExist()
    {
        GetUserCharacterRatingQuery.Handler handler = new(ActDb, Mapper);
        var result = await handler.Handle(new GetUserCharacterRatingQuery
        {
            CharacterId = 1,
            UserId = 2,
        }, CancellationToken.None);

        Assert.That(result.Errors![0].Code, Is.EqualTo(ErrorCode.CharacterNotFound));
    }

    [Test]
    public async Task ShouldReturnCharacterRating()
    {
        Character character = new()
        {
            Name = "toto",
            UserId = 2,
            Statistics = new List<CharacterStatistics>
            {
                {
                    new CharacterStatistics
                    {
                        Kills = 1,
                        Deaths = 2,
                        Assists = 3,
                        PlayTime = TimeSpan.FromSeconds(4),
                        GameMode = GameMode.CRPGBattle,
                        Rating = new CharacterRating
                        {
                            Value = 1,
                            Deviation = 2,
                            Volatility = 3,
                        },
                    }
                },
            },
        };
        ArrangeDb.Characters.Add(character);
        await ArrangeDb.SaveChangesAsync();

        GetUserCharacterRatingQuery.Handler handler = new(ActDb, Mapper);
        var result = await handler.Handle(new GetUserCharacterRatingQuery
        {
            CharacterId = character.Id,
            UserId = 2,
            GameMode = GameMode.CRPGBattle,
        }, CancellationToken.None);

        Assert.That(result.Errors, Is.Null);
        Assert.That(result.Data, Is.Not.Null);
    }
}
