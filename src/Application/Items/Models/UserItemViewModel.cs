using AutoMapper;
using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Items;

namespace Crpg.Application.Items.Models;

public record UserItemViewModel : IMapFrom<UserItem>
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public ItemViewModel Item { get; init; } = default!;
    public bool IsBroken { get; init; }
    public DateTime CreatedAt { get; init; }

    public bool IsArmoryItem { get; init; }
    public bool IsPersonal { get; init; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UserItem, UserItemViewModel>()
            .ForMember(ui => ui.IsArmoryItem, config => config.MapFrom(ui => ui.ClanArmoryItem != null))
            .ForMember(i => i.IsPersonal, opt => opt.MapFrom(ui => ui.Item!.PersonalItems.Any(pi => pi.UserId == ui.UserId)));
    }
}
