using Crpg.Application.Restrictions.Queries;
using Crpg.Domain.Entities.Restrictions;
using Crpg.Domain.Entities.Users;
using Crpg.Sdk;
using Crpg.Sdk.Abstractions;
using Moq;
using NUnit.Framework;

namespace Crpg.Application.UTest.Restrictions;

public class GetUserRestrictionQueryTest : TestBase
{
    [Test]
    public async Task Basic()
    {
        var now = new DateTime(2024, 1, 1, 12, 0, 0);
        User user = new()
        {
            Restrictions = new List<Restriction>
            {
                new() { RestrictedByUser = new User { PlatformUserId = "123" } },
                new() { RestrictedByUser = new User { PlatformUserId = "456" }, Type = RestrictionType.Join, Duration = TimeSpan.FromDays(10), CreatedAt = now.AddDays(-1), PublicReason = "toto", },
            },
        };
        ArrangeDb.Users.Add(user);
        await ArrangeDb.SaveChangesAsync();

        Mock<IDateTime> dateTime = new();
        dateTime
            .Setup(dt => dt.UtcNow)
            .Returns(now);

        var result = await new GetUserRestrictionQuery.Handler(ActDb, Mapper, dateTime.Object).Handle(
            new GetUserRestrictionQuery { UserId = user.Id }, CancellationToken.None);
        var restriction = result.Data!;
        Assert.That(restriction, Is.Not.Null);
        Assert.That(restriction.Reason, Is.EqualTo("toto"));
    }
}
