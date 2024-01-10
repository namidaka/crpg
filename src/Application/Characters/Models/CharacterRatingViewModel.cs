using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Characters;
using Crpg.Domain.Entities.Servers;

namespace Crpg.Application.Characters.Models;

public record CharacterRatingViewModel : IMapFrom<CharacterRating>
{
    public GameMode GameMode { get; set; }
    public float Value { get; init; }
    public float Deviation { get; init; }
    public float Volatility { get; init; }
    public float CompetitiveValue { get; init; }
}
