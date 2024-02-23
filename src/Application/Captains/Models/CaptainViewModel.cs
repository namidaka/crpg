using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Captains;

namespace Crpg.Application.Captains.Models;

public record CaptainViewModel : IMapFrom<Captain>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public IList<CaptainFormation> Formations { get; set; } = new List<CaptainFormation>();
    public bool ForTournament { get; init; }
}
