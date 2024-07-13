using Crpg.Domain.Common;
using Crpg.Domain.Entities.Users;

namespace Crpg.Domain.Entities.Items;

/// <summary>
/// Personal Item owned by a <see cref="User"/>.
/// </summary>
public class PersonalItem : AuditableEntity
{
    public int UserItemId { get; set; }

    public UserItem? UserItem { get; set; }
}
