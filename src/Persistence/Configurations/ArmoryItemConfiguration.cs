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

public class ArmoryItemConfiguration : IEntityTypeConfiguration<ArmoryItem>
{
    public void Configure(EntityTypeBuilder<ArmoryItem> builder)
    {
        builder.HasKey(e => e.UserItemId);

        builder.HasOne(e => e.Clan)
            .WithMany(e => e.ArmoryItems)
            .HasForeignKey(e => e.ClanId)
            .IsRequired();

        builder.HasOne(e => e.UserItem)
            .WithOne(e => e.ArmoryItem)
            .HasForeignKey<ArmoryItem>(e => e.UserItemId)
            .IsRequired();
    }
}

public class ArmoryBorrowConfiguration : IEntityTypeConfiguration<ArmoryBorrow>
{
    public void Configure(EntityTypeBuilder<ArmoryBorrow> builder)
    {
        builder.HasKey(e => e.UserItemId);

        builder.HasOne(e => e.Clan)
            .WithMany(e => e.ArmoryBorrows)
            .HasForeignKey(e => e.ClanId)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithMany(e => e.ArmoryBorrows)
            .HasForeignKey(e => e.UserId)
            .IsRequired();

        builder.HasOne(e => e.ArmoryItem)
            .WithOne(e => e.Borrow)
            .HasForeignKey<ArmoryBorrow>(e => e.UserItemId)
            .IsRequired();

        builder.HasOne(e => e.UserItem)
            .WithOne(e => e.ArmoryBorrow)
            .HasForeignKey<ArmoryBorrow>(e => e.UserItemId);
    }
}
