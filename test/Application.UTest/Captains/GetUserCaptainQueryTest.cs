using Crpg.Application.Captains.Commands;
using Crpg.Application.Captains.Queries;
using Crpg.Domain.Entities.Captains;
using NUnit.Framework;

namespace Crpg.Application.UTest.Captains;

public class GetUserCaptainQueryTest : TestBase
{
    [Test]
    public async Task ShouldGetCaptainIfExists()
    {
        Captain captain = new() { UserId = 0, Id = 0 };
        ArrangeDb.Captains.Add(captain);
        await ArrangeDb.SaveChangesAsync();

        var result = await new GetUserCaptainQuery.Handler(ActDb, Mapper).Handle(new GetUserCaptainQuery
        {
            UserId = captain.UserId,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Not.Null);
    }

    [Test]
    public async Task ShouldReturnErrorIfCaptainDoesntExist()
    {
        var result = await new GetUserCaptainQuery.Handler(ActDb, Mapper).Handle(new GetUserCaptainQuery
        {
           UserId = 1,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Null);
        Assert.That(result.Errors!, Is.Not.Empty);
    }
}
