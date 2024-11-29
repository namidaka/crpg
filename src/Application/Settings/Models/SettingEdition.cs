namespace Crpg.Application.Settlements.Models;

public record SettingEdition
{
    public string Discord { get; set; } = default!;
    public string Steam { get; set; } = default!;
    public string Patreon { get; set; } = default!;
    public string Github { get; set; } = default!;
    public string Reddit { get; set; } = default!;
    public string ModDb { get; set; } = default!;
}
