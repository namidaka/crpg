using AutoMapper;
using Crpg.Application.Common.Mappings;
using Crpg.Application.Users.Models;
using Crpg.Domain.Entities.Captains;

namespace Crpg.Application.Captains.Models;

public record CaptainViewModel : IMapFrom<Captain>
{
    public IList<CaptainFormationViewModel> Formations { get; set; } = new List<CaptainFormationViewModel>();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Captain, CaptainViewModel>()
            .ForMember(dest => dest.Formations, opt => opt.MapFrom(src => src.Formations.OrderBy(f => f.Id)));
    }
}
