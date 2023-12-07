using Crpg.Application.Characters.Commands;
using Crpg.Application.Common;
using Crpg.Application.Common.Services;
using Crpg.Domain.Entities.Characters;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Users;

public class RewardRecentUserCommandTest : TestBase
{
    private static readonly Constants Constants = new()
    {
        DefaultExperienceMultiplier = 1.0f,
        StartedCharacterLevel = 30,
    };

    [Test]
    public async Task ShouldRewardUserAndCharacter()
    {
        User user = new()
        {
            Gold = 0,
            ExperienceMultiplier = 1.0f,
            Characters = new List<Character> { new() { Level = 20, Experience = 2000 }, new() { Level = 22, Experience = 3000 } },
        };
        ArrangeDb.Users.Add(user);
        await ArrangeDb.SaveChangesAsync();

        Mock<IExperienceTable> experienceTableMock = new();
        experienceTableMock.Setup(et => et.GetLevelForExperience(Constants.StartedCharacterLevel)).Returns(10000);
        Mock<ICharacterService> characterServiceMock = new();

        RewardRecentUserCommand.Handler handler = new(ActDb, Constants, characterServiceMock.Object,
        experienceTableMock.Object);
        await handler.Handle(new RewardRecentUserCommand { }, CancellationToken.None);

        var characterDb = await AssertDb.Characters
            .Include(c => c.User)
            .FirstAsync(c => c.UserId == user.Id);

        Assert.That(characterDb.User!.Gold, Is.EqualTo(user.Gold + 25000));
        characterServiceMock.Verify(cs => cs.GiveExperience(It.IsAny<Character>(), It.IsAny<int>(), false), Times.Once());
    }

    [Test]
    public async Task ShouldNotRewardUserCozExpMultiNotEqualDefault()
    {
        User user = new()
        {
            Gold = 0,
            ExperienceMultiplier = 1.03f,
            Characters = new List<Character> { new() { Level = 20, Experience = 2000 }, new() { Level = 22, Experience = 3000 } },
        };
        ArrangeDb.Users.Add(user);
        await ArrangeDb.SaveChangesAsync();

        Mock<IExperienceTable> experienceTableMock = new();
        experienceTableMock.Setup(et => et.GetLevelForExperience(Constants.StartedCharacterLevel)).Returns(10000);
        Mock<ICharacterService> characterServiceMock = new();

        RewardRecentUserCommand.Handler handler = new(ActDb, Constants, characterServiceMock.Object,
        experienceTableMock.Object);
        await handler.Handle(new RewardRecentUserCommand { }, CancellationToken.None);

        var characterDb = await AssertDb.Characters
            .Include(c => c.User)
            .FirstAsync(c => c.UserId == user.Id);

        Assert.That(characterDb.User!.Gold, Is.EqualTo(user.Gold));
        characterServiceMock.Verify(cs => cs.GiveExperience(It.IsAny<Character>(), It.IsAny<int>(), false), Times.Never());
    }

    [Test]
    public async Task ShouldNotRewardUserCozHasHighLevelChar()
    {
        User user = new()
        {
            Gold = 0,
            ExperienceMultiplier = 1.0f,
            Characters = new List<Character> { new() { Level = 30, Experience = 2000 }, new() { Level = 22, Experience = 3000 } },
        };
        ArrangeDb.Users.Add(user);
        await ArrangeDb.SaveChangesAsync();

        Mock<IExperienceTable> experienceTableMock = new();
        experienceTableMock.Setup(et => et.GetLevelForExperience(Constants.StartedCharacterLevel)).Returns(10000);
        Mock<ICharacterService> characterServiceMock = new();

        RewardRecentUserCommand.Handler handler = new(ActDb, Constants, characterServiceMock.Object,
        experienceTableMock.Object);
        await handler.Handle(new RewardRecentUserCommand { }, CancellationToken.None);

        var characterDb = await AssertDb.Characters
            .Include(c => c.User)
            .FirstAsync(c => c.UserId == user.Id);

        Assert.That(characterDb.User!.Gold, Is.EqualTo(user.Gold));
        characterServiceMock.Verify(cs => cs.GiveExperience(It.IsAny<Character>(), It.IsAny<int>(), false), Times.Never());
    }

    [Test]
    public async Task ShouldNotRewardUserCozHasManyChars()
    {
        User user = new()
        {
            Gold = 0,
            ExperienceMultiplier = 1.0f,
            Characters = new List<Character>
            {
                new() { Level = 29, Experience = 4000000 },
                new() { Level = 29, Experience = 4000000 },
                new() { Level = 29, Experience = 4000000 },
            },
        };
        ArrangeDb.Users.Add(user);
        await ArrangeDb.SaveChangesAsync();

        Mock<IExperienceTable> experienceTableMock = new();
        experienceTableMock.Setup(et => et.GetLevelForExperience(Constants.StartedCharacterLevel)).Returns(10000);
        Mock<ICharacterService> characterServiceMock = new();

        RewardRecentUserCommand.Handler handler = new(ActDb, Constants, characterServiceMock.Object,
        experienceTableMock.Object);
        await handler.Handle(new RewardRecentUserCommand { }, CancellationToken.None);

        var characterDb = await AssertDb.Characters
            .Include(c => c.User)
            .FirstAsync(c => c.UserId == user.Id);

        Assert.That(characterDb.User!.Gold, Is.EqualTo(user.Gold));
        characterServiceMock.Verify(cs => cs.GiveExperience(It.IsAny<Character>(), It.IsAny<int>(), false), Times.Never());
    }
}
