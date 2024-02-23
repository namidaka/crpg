using Crpg.Domain.Common;
using Crpg.Domain.Entities.Characters;

namespace Crpg.Domain.Entities.Captains;

/// <summary>
/// Represents a cRPG captain formation.
/// </summary>
public class CaptainFormation : AuditableEntity
{
    public int UserId { get; set; }
    public int Id { get; set; }

    /// <summary>
    /// The troops to spawn in a formation .
    /// </summary>
    public Character? Troop { get; set; }

    /// <summary>
    /// The weight is compared to other formations to determine the composition of an army.
    /// </summary>
    public float Weight { get; set; }
}
