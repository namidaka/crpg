using AutoMapper;
using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Restrictions;

namespace Crpg.Application.Restrictions.Models;

public record RestrictionPublicViewModel : IMapFrom<Restriction>
{
    public int Id { get; init; }
    public TimeSpan Duration { get; init; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Restriction, RestrictionPublicViewModel>()
            .ForMember(r => r.Reason, opt => opt.MapFrom(src => src.PublicReason));
    }
}
