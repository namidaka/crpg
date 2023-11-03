using Crpg.Domain.Entities.Clans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class ClanArmoryBorrowConfiguration : IEntityTypeConfiguration<ClanArmoryBorrow>
{
    public void Configure(EntityTypeBuilder<ClanArmoryBorrow> builder)
    {
        builder.HasKey(e => e.UserItemId);

        builder.HasOne(e => e.ClanMember)
            .WithMany(e => e.ArmoryBorrows)
            .HasForeignKey(e => e.UserId)
            .IsRequired();

        builder.HasOne(e => e.ArmoryItem)
            .WithOne(e => e.Borrow)
            .HasForeignKey<ClanArmoryBorrow>(e => e.UserItemId)
            .IsRequired();

        builder.HasOne(e => e.UserItem)
            .WithOne(e => e.ClanArmoryBorrow)
            .HasForeignKey<ClanArmoryBorrow>(e => e.UserItemId)
            .IsRequired();

        builder.HasOne(e => e.Clan)
            .WithMany(e => e.ArmoryBorrows)
            .HasForeignKey(e => e.ClanId)
            .IsRequired();
    }
}
