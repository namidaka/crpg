using Crpg.Domain.Entities.GameServers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;
public class GameServerConfiguration : IEntityTypeConfiguration<GameServerConfig>
{
    public void Configure(EntityTypeBuilder<GameServerConfig> builder)
    {
        builder.HasKey(g => g.Id);
        builder
            .HasMany(g => g.Maps)
            .WithOne()
            .HasForeignKey("GameServerConfigId");
    }
}
