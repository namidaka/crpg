using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Characters;
using Crpg.Domain.Entities.Servers;

namespace Crpg.Application.Characters.Models;

public record CharacterStatisticsViewModel : IMapFrom<CharacterStatistics>
{
    public int Kills { get; init; }
    public int Deaths { get; init; }
    public int Assists { get; init; }
    public TimeSpan PlayTime { get; init; }
    public GameMode GameMode { get; init; }
    public CharacterRatingViewModel Rating { get; init; } = new();
}
