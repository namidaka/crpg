using Crpg.Application.Captains.Commands;
using Crpg.Application.Captains.Queries;
using Crpg.Domain.Entities.Captains;
using Crpg.Domain.Entities.Characters;
using NUnit.Framework;

namespace Crpg.Application.UTest.Captains;

public class UpdateFormationWeightCommandTest : TestBase
{
    [Test]
    public async Task ShouldUpdateIfFormationExists()
    {
        Captain captain = new()
        {
            UserId = 1,
            Formations = new List<CaptainFormation>() { new() { Number = 1, Weight = 33 } },
        };

        ArrangeDb.Captains.Add(captain);
        await ArrangeDb.SaveChangesAsync();

        var result = await new UpdateFormationWeightCommand.Handler(ActDb, Mapper).Handle(new UpdateFormationWeightCommand
        {
            UserId = captain.UserId,
            Weight = 50,
            Number = 1,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Weight, Is.EqualTo(50));
    }

    [Test]
    public async Task ShouldReturnErrorIfFormationDoesNotExist()
    {
        var result = await new UpdateFormationWeightCommand.Handler(ActDb, Mapper).Handle(new UpdateFormationWeightCommand
        {
            UserId = 1,
            Weight = 50,
            Number = 1,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Null);
        Assert.That(result.Errors!, Is.Not.Empty);
    }

}
