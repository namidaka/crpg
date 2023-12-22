using Crpg.Domain.Entities;
using Crpg.Domain.Entities.Servers;

namespace Crpg.Application.Games.Models;

public record GameServerStats
{
    public GameStats Total { get; set; } = default!;
    public Dictionary<Region, Dictionary<GameMode, GameStats>> Regions { get; set; } = default!;
}

public record GameStats
{
    /// <summary>Number of users currently playing on servers.</summary>
    public int PlayingCount { get; set; }
}
