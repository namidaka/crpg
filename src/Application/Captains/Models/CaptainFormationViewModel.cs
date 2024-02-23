using AutoMapper;
using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Captains;
using Crpg.Domain.Entities.Characters;

namespace Crpg.Application.Captains.Models;

public record CaptainFormationViewModel : IMapFrom<CaptainFormation>
{
    public int Id { get; set; }
    public Character? Troop { get; set; }
    public float Weight { get; set; }
    public CaptainViewModel Captain { get; set; } = default!;

}
