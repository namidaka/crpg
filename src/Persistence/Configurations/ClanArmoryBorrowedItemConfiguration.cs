using Crpg.Domain.Entities.Clans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class ClanArmoryBorrowedItemConfiguration : IEntityTypeConfiguration<ClanArmoryBorrowedItem>
{
    public void Configure(EntityTypeBuilder<ClanArmoryBorrowedItem> builder)
    {
        builder.HasKey(bi => bi.UserItemId);

        builder.HasOne(bi => bi.Borrower)
            .WithMany(cm => cm.ArmoryBorrowedItems)
            .HasForeignKey(bi => bi.BorrowerUserId)
            .IsRequired();

        builder.HasOne(bi => bi.ArmoryItem)
            .WithOne(ci => ci.BorrowedItem)
            .HasForeignKey<ClanArmoryBorrowedItem>(bi => bi.UserItemId)
            .IsRequired();

        builder.HasOne(bi => bi.UserItem)
            .WithOne(ui => ui.ClanArmoryBorrowedItem)
            .HasForeignKey<ClanArmoryBorrowedItem>(bi => bi.UserItemId)
            .IsRequired();

        builder.HasOne(bi => bi.Clan)
            .WithMany(c => c.ArmoryBorrowedItems)
            .HasForeignKey(bi => bi.BorrowerClanId)
            .IsRequired();
    }
}
