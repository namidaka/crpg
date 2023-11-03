using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Crpg.Application.Clans.Models;
using Crpg.Application.Common.Mappings;
using Crpg.Application.Users.Models;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;

namespace Crpg.Application.Items.Models;
public class ClanArmoryItemViewModel : IMapFrom<ClanArmoryItem>
{
    public UserItemViewModel? UserItem { get; set; }
    public ClanArmoryBorrowViewModel? Borrow { get; set; }
    public DateTime UpdatedAt { get; set; }
}
