using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Captains;
using Crpg.Domain.Entities.Characters;

namespace Crpg.Application.Captains.Models;

public record CaptainFormationViewModel : IMapFrom<CaptainFormation>
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public Character? Troop { get; set; }
    public float Weight { get; set; }
}
