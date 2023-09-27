using System;
using System.Collections.Generic;
using System.Text;
using Crpg.Domain.Common;
using Crpg.Domain.Entities.Servers.Enums;

namespace Crpg.Domain.Entities.Servers;
public class GameServerConfig : AuditableEntity
{
    public ServerIdentifier Identifier { get; set; }
    public string? ServerName { get; set; }
    public string? GameType { get; set; }
    public string? GamePassword { get; set; }
    public string? WelcomeMessage { get; set; }
    public List<string> Maps { get; set; } = new List<string>();
    public bool? AllowPollsToKickPlayers { get; set; }
    public bool? AllowPollsToChangeMaps { get; set; }
    public string? DisableCultureVoting { get; set; }
}
