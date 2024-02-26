using Crpg.Application.Captains.Commands;
using Crpg.Application.Captains.Queries;
using Crpg.Domain.Entities.Captains;
using Crpg.Domain.Entities.Clans;
using Crpg.Domain.Entities.Users;
using NUnit.Framework;

namespace Crpg.Application.UTest.Captains;

public class GetUserCaptainCommandTest : TestBase
{
    [Test]
    public async Task ShouldGetCaptainIfExists()
    {
        User user = new();
        ArrangeDb.Users.Add(user);

        Captain captain = new() { UserId = user.Id };
        ArrangeDb.Captains.Add(captain);
        await ArrangeDb.SaveChangesAsync();

        var result = await new GetUserCaptainCommand.Handler(ActDb, Mapper).Handle(new GetUserCaptainCommand
        {
            UserId = captain.UserId,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Not.Null);
    }

    [Test]
    public async Task ShouldCreateCaptainAndFormationsIfCaptainDoesntExist()
    {
        User user = new();
        ArrangeDb.Users.Add(user);
        await ArrangeDb.SaveChangesAsync();

        var result = await new GetUserCaptainCommand.Handler(ActDb, Mapper).Handle(new GetUserCaptainCommand
        {
            UserId = user.Id,
        }, CancellationToken.None);

        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Formations?.Count, Is.EqualTo(3));
    }
}
