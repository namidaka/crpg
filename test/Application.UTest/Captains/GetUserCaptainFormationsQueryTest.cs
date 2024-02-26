using Crpg.Application.Captains.Commands;
using Crpg.Application.Captains.Queries;
using Crpg.Domain.Entities.Captains;
using NUnit.Framework;

namespace Crpg.Application.UTest.Captains;

public class GetUserCaptainFormationsQueryTest : TestBase
{
    [Test]
    public async Task ShouldGetFormationsIfExists()
    {
        Captain captain = new()
        {
            UserId = 1,
            Formations = new List<CaptainFormation>() { new() { Number = 1, Weight = 33 }, new() { Number = 2, Weight = 33 } },
        };
        ArrangeDb.Captains.Add(captain);
        await ArrangeDb.SaveChangesAsync();

        var result = await new GetUserCaptainFormationsQuery.Handler(ActDb, Mapper).Handle(new GetUserCaptainFormationsQuery
        {
            UserId = captain.UserId,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task ShouldReturnErrorIfFormationsDoNotExist()
    {
        var result = await new GetUserCaptainQuery.Handler(ActDb, Mapper).Handle(new GetUserCaptainQuery
        {
            UserId = 1,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Null);
        Assert.That(result.Errors!, Is.Not.Empty);
    }
}
