using Crpg.Domain.Common;
using Crpg.Domain.Entities.Characters;

namespace Crpg.Domain.Entities.Captains;

/// <summary>
/// Represents a cRPG captain formation.
/// </summary>
public class CaptainFormation : AuditableEntity
{
    public int Id { get; set; }
    /// <summary>
    /// The number indentifies the formation slot of the captain.
    /// </summary>
    public int Number { get; set; }
    public int CaptainId { get; set; }
    /// <summary>
    /// The characterId of the troops to spawn in a formation.
    /// </summary>
    public int? CharacterId { get; set; }

    /// <summary>
    /// The weight is compared to other formations to determine the composition of an army.
    /// </summary>
    public int Weight { get; set; }

    public Captain Captain { get; set; } = default!;
    public Character? Character { get; set; }
}
