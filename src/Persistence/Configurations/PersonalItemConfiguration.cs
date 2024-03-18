using Crpg.Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class PersonalItemConfiguration : IEntityTypeConfiguration<PersonalItem>
{
    public void Configure(EntityTypeBuilder<PersonalItem> builder)
    {
        builder.HasIndex(pi => new { pi.UserId, pi.ItemId }).IsUnique();

        builder.HasKey(pi => new { pi.UserId, pi.ItemId });
    }
}
