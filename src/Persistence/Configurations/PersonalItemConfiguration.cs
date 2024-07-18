using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class PersonalItemConfiguration : IEntityTypeConfiguration<PersonalItem>
{
    public void Configure(EntityTypeBuilder<PersonalItem> builder)
    {
        builder.HasKey(pi => pi.UserItemId);

        builder.HasOne(pi => pi.UserItem)
            .WithOne(ui => ui.PersonalItem)
            .HasForeignKey<PersonalItem>(ci => ci.UserItemId)
            .IsRequired();
    }
}
