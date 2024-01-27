using Crpg.Application.Common.Results;
using Crpg.Application.Users.Commands;
using Crpg.Domain.Entities.Users;
using NUnit.Framework;

namespace Crpg.Application.UTest.Users;

public class UpdateUserNoteCommandTest : TestBase
{
    [Test]
    public async Task ShouldUpdateUserNote()
    {
        var user = ArrangeDb.Users.Add(new User());
        await ArrangeDb.SaveChangesAsync();

        UpdateUserNoteCommand cmd = new()
        {
            UserId = user.Entity.Id,
            Note = "toto",
        };

        var res = await new UpdateUserNoteCommand.Handler(ActDb, Mapper).Handle(cmd, CancellationToken.None);

        Assert.That(res.Errors, Is.Null);
        Assert.That(res.Data!.Note, Is.EqualTo(cmd.Note));
    }

    [Test]
    public async Task UserNotFound()
    {
        ArrangeDb.Users.Add(new User());
        await ArrangeDb.SaveChangesAsync();

        UpdateUserNoteCommand cmd = new()
        {
            UserId = 12,
        };

        var res = await new UpdateUserNoteCommand.Handler(ActDb, Mapper).Handle(cmd, CancellationToken.None);

        Assert.That(res.Errors![0].Code, Is.EqualTo(ErrorCode.UserNotFound));
    }
}
