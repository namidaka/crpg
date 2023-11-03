using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Crpg.Application.Common.Mappings;
using Crpg.Application.Items.Models;
using Crpg.Application.Users.Models;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;

namespace Crpg.Application.Clans.Models;
public class ArmoryItemViewModel : IMapFrom<ArmoryItem>
{
    public UserItemViewModel? UserItem { get; set; }
    public ArmoryBorrowViewModel? Borrow { get; set; }
    public DateTime UpdatedAt { get; set; }
}
