using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Clans;

namespace Crpg.Application.Clans.Models;
public class ClanArmoryBorrowViewModel : IMapFrom<ClanArmoryBorrow>
{
    public int UserId { get; set; }
    public int UserItemId { get; set; }
    public DateTime UpdatedAt { get; set; }
}
