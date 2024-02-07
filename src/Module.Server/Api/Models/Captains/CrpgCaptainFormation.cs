using Crpg.Module.Api.Models.Characters;

namespace Crpg.Module.Api.Models.Captains;
internal class CrpgCaptainFormation
{
    public int Id { get; set; }
    public int Number { get; set; }
    public CrpgCharacter? Character { get; set; } = default!;
    public int Weight { get; set; }
}
