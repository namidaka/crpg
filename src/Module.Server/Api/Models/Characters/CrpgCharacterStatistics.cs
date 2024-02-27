using Crpg.Domain.Entities.Servers;

namespace Crpg.Module.Api.Models.Characters;

// Copy of Crpg.Application.Characters.Model.CharacterStatisticsViewModel
internal class CrpgCharacterStatistics
{
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
    public TimeSpan PlayTime { get; set; }
    public GameMode GameMode { get; set; }
    public CrpgCharacterRating Rating { get; set; } = new();
}
