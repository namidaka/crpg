﻿using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Settings;

namespace Crpg.Application.Settlements.Models;

public record SettingViewModel : IMapFrom<Setting>
{
    public string Discord { get; set; } = default!;
    public string Steam { get; set; } = default!;
    public string Patreon { get; set; } = default!;
    public string Github { get; set; } = default!;
    public string Reddit { get; set; } = default!;
    public string ModDb { get; set; } = default!;
}
