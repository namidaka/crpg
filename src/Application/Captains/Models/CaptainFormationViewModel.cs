using AutoMapper;
using Crpg.Application.Characters.Models;
using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Captains;
using Crpg.Domain.Entities.Characters;

namespace Crpg.Application.Captains.Models;

public record CaptainFormationViewModel : IMapFrom<CaptainFormation>
{
    public int Number { get; set; }
    public int? CharacterId { get; set; }
    public GameCharacterViewModel? Character { get; set; }
    public int Weight { get; set; }

}
