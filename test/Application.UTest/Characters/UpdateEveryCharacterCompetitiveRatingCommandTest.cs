using Crpg.Application.Characters.Commands;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Characters;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Characters;

public class UpdateEveryCharacterCompetitiveRatingCommandTest : TestBase
{
    [Test]
    public async Task Basic()
    {
        Character character0 = new();
        character0.Statistics.Add(new CharacterStatistics { Rating = new CharacterRating { CompetitiveValue = 10, Value = 1, Deviation = 1, Volatility = 1 } });
        Character character1 = new();
        character1.Statistics.Add(new CharacterStatistics { Rating = new CharacterRating { CompetitiveValue = 10, Value = 1, Deviation = 1, Volatility = 1 } });
        ArrangeDb.Characters.AddRange(character0, character1);
        await ArrangeDb.SaveChangesAsync();

        Mock<ICompetitiveRatingModel> competitiveRatingModel = new();
        UpdateEveryCharacterCompetitiveRatingCommand.Handler handler = new(ActDb, competitiveRatingModel.Object);
        await handler.Handle(new UpdateEveryCharacterCompetitiveRatingCommand(), CancellationToken.None);

        competitiveRatingModel.Verify(m => m.ComputeCompetitiveRating(It.IsAny<CharacterRating>()), Times.Exactly(2));
    }
}
