using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class UserItemConfiguration : IEntityTypeConfiguration<UserItem>
{
    public void Configure(EntityTypeBuilder<UserItem> builder)
    {
        builder.HasIndex(ui => new { ui.UserId, ui.ItemId }).IsUnique();

        builder.HasOne(ui => ui.User)
            .WithMany(u => u.Items)
            .HasForeignKey(ui => ui.UserId)
            .IsRequired();

        builder.HasOne(ui => ui.Item)
            .WithMany(i => i.UserItems)
            .HasForeignKey(ui => ui.ItemId)
            .IsRequired();
    }
}
