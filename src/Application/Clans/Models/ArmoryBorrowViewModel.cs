using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Crpg.Application.Common.Mappings;
using Crpg.Domain.Entities.Items;

namespace Crpg.Application.Clans.Models;
public class ArmoryBorrowViewModel : IMapFrom<ArmoryBorrow>
{
    public int UserId { get; set; }
    public int UserItemId { get; set; }
    public DateTime UpdatedAt { get; set; }
}
