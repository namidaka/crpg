using Crpg.Domain.Common;
using Crpg.Domain.Entities.Servers.Enums;

namespace Crpg.Domain.Entities.Servers;
public class GameServerConfig : AuditableEntity
{
    // <Information> Common settings for all type of server <Information>
    public ServerIdentifier Identifier { get; set; }
    public string? ServerName { get; set; }
    public GameType GameType { get; set; }
    public string? GamePassword { get; set; }
    public string? WelcomeMessage { get; set; }
    public List<string> Maps { get; set; } = new List<string>();
    public bool? AllowPollsToKickPlayers { get; set; }
    public bool? AllowPollsToChangeMaps { get; set; }
    public bool? DisableCultureVoting { get; set; }
    public Culture CultureTeam1 { get; set; }
    public Culture CultureTeam2 { get; set; }
    public int MaxNumberOfPlayers { get; set; }
    public int? AutoTeamBalanceThreshold { get; set; }
    public int? FriendlyFireDamageMeleeFriendPercent { get; set; }
    public int? FriendlyFireDamageMeleeSelfPercent { get; set; }
    public int? FriendlyFireDamageRangedFriendPercent { get; set; }
    public int? FriendlyFireDamageRangedSelfPercent { get; set; }
    public int NumberOfBotsTeam1 { get; set; }
    public int NumberOfBotsTeam2 { get; set; }
    public int? MinNumberOfPlayersForMatchStart { get; set; } // Battle, Siege, TDeathmatch
    public int? RoundTotal { get; set; }
    public int? MapTimeLimit { get; set; }
    public int? RoundTimeLimit { get; set; }
    public int? WarmupTimeLimit { get; set; }
    public int? RoundPreparationTimeLimit { get; set; } // Battle, Conquest, DTV, Siege, Deathmatch
    public bool? Enable_automated_battle_switching { get; set; }

    // <Information> Conquest type of server settings <Information>
    public int? RespawnPeriodTeam1 { get; set; }
    public int? RespawnPeriodTeam2 { get; set; }
    public int? Crpg_reward_tick { get; set; }
    public bool? Set_automated_battle_count { get; set; }

    // <Information> Duel type of server settings <Information>
    public int? MinScoreToWinDuel { get; set; }
    public bool? End_game_after_mission_is_over { get; set; }
}
