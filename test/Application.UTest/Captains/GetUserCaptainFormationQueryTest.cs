using Crpg.Application.Captains.Commands;
using Crpg.Application.Captains.Queries;
using Crpg.Domain.Entities.Captains;
using NUnit.Framework;

namespace Crpg.Application.UTest.Captains;

public class GetUserCaptainFormationQueryTest : TestBase
{
    [Test]
    public async Task ShouldGetFormationsIfExists()
    {
        Captain captain = new()
        {
            UserId = 1,
            Formations = new List<CaptainFormation>() { new() { Number = 2, Weight = 33 } },
        };
        ArrangeDb.Captains.Add(captain);
        await ArrangeDb.SaveChangesAsync();

        var result = await new GetUserCaptainFormationQuery.Handler(ActDb, Mapper).Handle(new GetUserCaptainFormationQuery
        {
            UserId = captain.UserId,
            Number = 2,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Number, Is.EqualTo(2));
    }

    [Test]
    public async Task ShouldReturnErrorIfFormationsDoNotExist()
    {
        var result = await new GetUserCaptainFormationQuery.Handler(ActDb, Mapper).Handle(new GetUserCaptainFormationQuery
        {
            UserId = 1,
            Number = 2,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Null);
        Assert.That(result.Errors!, Is.Not.Empty);
    }
}
