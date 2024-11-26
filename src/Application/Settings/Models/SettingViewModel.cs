using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Settings;

namespace Crpg.Application.Settlements.Models;

public record SettingViewModel : IMapFrom<Setting>
{
    public int Id { get; set; }
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? Description { get; set; }
    public SettingDataType DataType { get; set; }
}
