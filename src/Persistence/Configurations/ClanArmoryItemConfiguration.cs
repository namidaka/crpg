using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class ClanArmoryItemConfiguration : IEntityTypeConfiguration<ClanArmoryItem>
{
    public void Configure(EntityTypeBuilder<ClanArmoryItem> builder)
    {
        builder.HasKey(ci => ci.UserItemId);

        builder.HasOne(ci => ci.Lender)
            .WithMany(cm => cm.ArmoryItems)
            .HasForeignKey(ci => ci.LenderUserId)
            .IsRequired();

        builder.HasOne(ci => ci.UserItem)
            .WithOne(ui => ui.ClanArmoryItem)
            .HasForeignKey<ClanArmoryItem>(ci => ci.UserItemId)
            .IsRequired();

        builder.HasOne(ci => ci.Clan)
            .WithMany(c => c.ArmoryItems)
            .HasForeignKey(ci => ci.LenderClanId)
            .IsRequired();
    }
}
