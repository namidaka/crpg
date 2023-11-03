using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class UserItemConfiguration : IEntityTypeConfiguration<UserItem>
{
    public void Configure(EntityTypeBuilder<UserItem> builder)
    {
        builder.HasIndex(ui => new { ui.UserId, ui.ItemId }).IsUnique();

        builder.HasOne(e => e.User)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.UserId)
            .IsRequired();

        builder.HasOne(e => e.Item)
            .WithMany(e => e.UserItems)
            .HasForeignKey(e => e.ItemId)
            .IsRequired();
    }
}
