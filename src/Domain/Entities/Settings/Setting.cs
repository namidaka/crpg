using Crpg.Domain.Common;

namespace Crpg.Domain.Entities.Settings;

/// <summary>
/// TODO:
/// </summary>
public class Setting : AuditableEntity
{
    public int Id { get; set; }
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? Description { get; set; }
    public SettingDataType DataType { get; set; }
}
