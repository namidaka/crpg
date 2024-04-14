using Crpg.Domain.Common;
using Crpg.Domain.Entities.Users;

namespace Crpg.Domain.Entities.Items;

/// <summary>
/// Personal Item owned by a <see cref="User"/>.
/// </summary>
public class PersonalItem : AuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public User? User { get; set; }
    public Item? Item { get; set; }
}
