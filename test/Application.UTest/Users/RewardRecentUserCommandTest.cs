using Crpg.Application.Characters.Commands;
using Crpg.Application.Common;
using Crpg.Application.Common.Results;
using Crpg.Application.Common.Services;
using Crpg.Application.Users.Commands;
using Crpg.Domain.Entities.Users;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Users;

public class RewardRecentUserCommandTest : TestBase
{
    private static readonly Constants Constants = new()
    {
    };

    [Test]
    public async Task Basic()
    {
        User user = new()
        {
            Gold = 200,
            HeirloomPoints = 1,
        };
        ArrangeDb.Users.Add(user);
        await ArrangeDb.SaveChangesAsync();

        Mock<IExperienceTable> experienceTableMock = new();
        Mock<ICharacterService> characterServiceMock = new();

        RewardRecentUserCommand.Handler handler = new(ActDb, Constants, characterServiceMock.Object,
        experienceTableMock.Object);
        await handler.Handle(new RewardRecentUserCommand { }, CancellationToken.None);

        // character = await AssertDb.Characters
        //     .Include(c => c.User)
        //     .FirstAsync(c => c.Id == character.Id);

        // Assert.That(res.Errors, Is.Null);
        // Assert.That(res.Data!.Gold, Is.EqualTo(300));
        // Assert.That(res.Data.HeirloomPoints, Is.EqualTo(3));
    }
}
