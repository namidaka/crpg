using AutoMapper;
using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Characters;

namespace Crpg.Application.Characters.Models;

public record CharacterRatingViewModel : IMapFrom<CharacterRating>
{
    public float Value { get; init; }
    public float Deviation { get; init; }
    public float Volatility { get; init; }
    public float CompetitiveValue { get; init; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CharacterRating, CharacterRatingViewModel>()
            .ForMember(r => r.Value, opt => opt.MapFrom(r => r.Value))
            .ForMember(r => r.Deviation, opt => opt.MapFrom(r => r.Deviation))
            .ForMember(r => r.Volatility, opt => opt.MapFrom(r => r.Volatility))
            .ForMember(r => r.CompetitiveValue, opt => opt.MapFrom(r => r.CompetitiveValue));
    }
}
