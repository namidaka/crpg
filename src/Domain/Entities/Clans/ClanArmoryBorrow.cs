using Crpg.Domain.Common;
using Crpg.Domain.Entities.Clans;
using Crpg.Domain.Entities.Users;

namespace Crpg.Domain.Entities.Items;
public class ClanArmoryBorrow : AuditableEntity
{
    public int ClanId { get; set; }
    public int UserId { get; set; }
    public int UserItemId { get; set; }

    public User? User { get; set; }
    public Clan? Clan { get; set; }
    public ClanArmoryItem? ClanArmoryItem { get; set; }
    public UserItem? UserItem { get; set; }
}
