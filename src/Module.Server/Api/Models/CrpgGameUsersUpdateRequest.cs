using Crpg.Domain.Entities.Servers;

namespace Crpg.Module.Api.Models;

// Copy of Crpg.Application.Games.Commands.UpdateGameUsersCommand
internal class CrpgGameUsersUpdateRequest
{
    public IList<CrpgUserUpdate> Updates { get; set; } = Array.Empty<CrpgUserUpdate>();
    public GameMode GameMode { get; set; } = default!;
}
