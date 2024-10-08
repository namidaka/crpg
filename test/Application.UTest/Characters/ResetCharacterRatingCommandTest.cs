using Crpg.Application.Characters.Commands;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Characters;
using Crpg.Domain.Entities.Users;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Characters;

public class ResetCharacterRatingCommandTest : TestBase
{
    [Test]
    public async Task Basic()
    {
        Character character = new()
        {
            User = new User(),
        };
        ArrangeDb.Add(character);
        await ArrangeDb.SaveChangesAsync();

        Mock<ICharacterService> characterServiceMock = new();
        Mock<IActivityLogService> activityLogServiceMock = new() { DefaultValue = DefaultValue.Mock };

        ResetCharacterRatingCommand.Handler handler = new(ActDb, Mapper, characterServiceMock.Object,
            activityLogServiceMock.Object);
        await handler.Handle(new ResetCharacterRatingCommand
        {
            CharacterId = character.Id,
            UserId = character.UserId,
        }, CancellationToken.None);

        characterServiceMock.Verify(cs => cs.ResetAllRatings(It.IsAny<Character>()));
    }

    [Test]
    public async Task NotFoundIfUserDoesntExist()
    {
        var characterService = Mock.Of<ICharacterService>();
        var activityLogService = Mock.Of<IActivityLogService>();
        ResetCharacterRatingCommand.Handler handler = new(ActDb, Mapper, characterService, activityLogService);
        var result = await handler.Handle(
            new ResetCharacterRatingCommand
            {
                CharacterId = 1,
                UserId = 2,
            }, CancellationToken.None);
        Assert.That(result.Errors![0].Code, Is.EqualTo(ErrorCode.CharacterNotFound));
    }

    [Test]
    public async Task NotFoundIfCharacterDoesntExist()
    {
        var user = ArrangeDb.Users.Add(new User());
        await ArrangeDb.SaveChangesAsync();

        var characterService = Mock.Of<ICharacterService>();
        var activityLogService = Mock.Of<IActivityLogService>();
        ResetCharacterRatingCommand.Handler handler = new(ActDb, Mapper, characterService, activityLogService);
        var result = await handler.Handle(
            new ResetCharacterRatingCommand
            {
                CharacterId = 1,
                UserId = user.Entity.Id,
            }, CancellationToken.None);
        Assert.That(result.Errors![0].Code, Is.EqualTo(ErrorCode.CharacterNotFound));
    }
}
