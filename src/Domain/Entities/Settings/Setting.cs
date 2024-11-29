using Crpg.Domain.Common;

namespace Crpg.Domain.Entities.Settings;

/// <summary>
/// TODO:
/// </summary>
public class Setting
{
    public int Id { get; set; } = 1;
    public string Discord { get; set; } = default!;
    public string Steam { get; set; } = default!;
    public string Patreon { get; set; } = default!;
    public string Github { get; set; } = default!;
    public string Reddit { get; set; } = default!;
    public string ModDb { get; set; } = default!;
}
