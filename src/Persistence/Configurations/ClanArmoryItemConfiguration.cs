using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Domain.Entities.Clans;
using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class ClanArmoryItemConfiguration : IEntityTypeConfiguration<ClanArmoryItem>
{
    public void Configure(EntityTypeBuilder<ClanArmoryItem> builder)
    {
        builder.HasKey(e => e.UserItemId);

        builder.HasOne(e => e.Clan)
            .WithMany(e => e.ArmoryItems)
            .HasForeignKey(e => e.ClanId)
            .IsRequired();

        builder.HasOne(e => e.UserItem)
            .WithOne(e => e.ClanArmoryItem)
            .HasForeignKey<ClanArmoryItem>(e => e.UserItemId)
            .IsRequired();
    }
}
