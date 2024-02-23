using Crpg.Application.Common.Mappings;
using Crpg.Application.Users.Models;
using Crpg.Domain.Entities.Captains;

namespace Crpg.Application.Captains.Models;

public record CaptainViewModel : IMapFrom<Captain>
{
    public int Id { get; set; }
    public IList<CaptainFormation> Formations { get; set; } = new List<CaptainFormation>();
    public UserViewModel User { get; set; } = default!;
}
