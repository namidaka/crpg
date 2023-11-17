using Crpg.Domain.Entities.Clans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class ClanArmoryBorrowedItemConfiguration : IEntityTypeConfiguration<ClanArmoryBorrowedItem>
{
    public void Configure(EntityTypeBuilder<ClanArmoryBorrowedItem> builder)
    {
        builder.HasKey(e => e.UserItemId);

        builder.HasOne(e => e.Borrower)
            .WithMany(e => e.ArmoryBorrowedItems)
            .HasForeignKey(e => e.BorrowerUserId)
            .IsRequired();

        builder.HasOne(e => e.ArmoryItem)
            .WithOne(e => e.BorrowedItem)
            .HasForeignKey<ClanArmoryBorrowedItem>(e => e.UserItemId)
            .IsRequired();

        builder.HasOne(e => e.UserItem)
            .WithOne(e => e.ClanArmoryBorrowedItem)
            .HasForeignKey<ClanArmoryBorrowedItem>(e => e.UserItemId)
            .IsRequired();

        builder.HasOne(e => e.Clan)
            .WithMany(e => e.ArmoryBorrowedItems)
            .HasForeignKey(e => e.BorrowerClanId)
            .IsRequired();
    }
}
