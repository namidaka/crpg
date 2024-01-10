namespace Crpg.Module.Api.Models.Characters;

// Copy of Crpg.Application.Characters.Models.CharacterRatingViewModel
internal class CrpgCharacterRating
{
    public GameMode GameMode { get; set; }
    public float Value { get; set; }
    public float Deviation { get; set; }
    public float Volatility { get; set; }
    public float CompetitiveValue { get; set; }
}
