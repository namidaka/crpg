namespace Crpg.Module.Api.Models.Captains;

internal class CrpgCaptain
{
    public int Id { get; set; }
    public IList<CrpgCaptainFormation> Formations { get; set; } = new List<CrpgCaptainFormation>();
}
