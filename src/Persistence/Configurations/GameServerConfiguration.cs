using System.Reflection.Emit;
using Crpg.Domain.Entities.Servers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;
public class GameServerConfiguration : IEntityTypeConfiguration<GameServerConfig>
{
    public void Configure(EntityTypeBuilder<GameServerConfig> builder)
    {
        builder.HasKey(g => g.Id);
    }
}
