using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Clans;
using Crpg.Domain.Entities.Items;

namespace Crpg.Application.Clans.Models;
public class ClanArmoryBorrowUserViewModel : IMapFrom<ClanArmoryBorrow>
{
    public UserItem UserItem { get; set; } = default!;
    public DateTime UpdatedAt { get; set; }
}
