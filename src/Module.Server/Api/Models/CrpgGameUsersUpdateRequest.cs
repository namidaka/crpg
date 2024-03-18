namespace Crpg.Module.Api.Models;

// Copy of Crpg.Application.Games.Commands.UpdateGameUsersCommand
internal class CrpgGameUsersUpdateRequest
{
    public IList<CrpgUserUpdate> Updates { get; set; } = Array.Empty<CrpgUserUpdate>();
    public string Key { get; set; } = string.Empty;
}
