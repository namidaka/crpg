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

public class ClanArmoryBorrowConfiguration : IEntityTypeConfiguration<ClanArmoryBorrow>
{
    public void Configure(EntityTypeBuilder<ClanArmoryBorrow> builder)
    {
        builder.HasKey(e => e.UserItemId);

        builder.HasOne(e => e.Clan)
            .WithMany(e => e.ArmoryBorrows)
            .HasForeignKey(e => e.ClanId)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithMany(e => e.ClanArmoryBorrows)
            .HasForeignKey(e => e.UserId)
            .IsRequired();

        builder.HasOne(e => e.ClanArmoryItem)
            .WithOne(e => e.Borrow)
            .HasForeignKey<ClanArmoryBorrow>(e => e.UserItemId)
            .IsRequired();

        builder.HasOne(e => e.UserItem)
            .WithOne(e => e.ClanArmoryBorrow)
            .HasForeignKey<ClanArmoryBorrow>(e => e.UserItemId);
    }
}
