using Crpg.Domain.Entities;
using Crpg.Domain.Entities.Clans;
using Crpg.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class ClanConfiguration : IEntityTypeConfiguration<Clan>
{
    public void Configure(EntityTypeBuilder<Clan> builder)
    {
        builder.HasIndex(c => c.Tag).IsUnique();
        builder.HasIndex(c => c.Name).IsUnique();
        builder
            .Property(c => c.Languages)
            .HasConversion(new EnumListJsonValueConverter<Languages>());
    }
}
