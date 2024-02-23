using Crpg.Domain.Common;
using Crpg.Domain.Entities.Users;

namespace Crpg.Domain.Entities.Captains;

/// <summary>
/// Represents a cRPG captain.
/// </summary>
public class Captain : AuditableEntity
{
    public int UserId { get; set; }
    public IList<CaptainFormation> Formations { get; set; } = new List<CaptainFormation>();
    public User? User { get; set; }
}
