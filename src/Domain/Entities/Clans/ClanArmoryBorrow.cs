using System;
using System.Collections.Generic;
using System.Text;
using Crpg.Domain.Common;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;

namespace Crpg.Domain.Entities.Clans;
public class ClanArmoryBorrow : AuditableEntity
{
    public int ClanId { get; set; }
    public int UserId { get; set; }
    public int UserItemId { get; set; }

    public User? User { get; set; }
    public Clan? Clan { get; set; }
    public ClanArmoryItem? ArmoryItem { get; set; }
    public UserItem? UserItem { get; set; }
}
