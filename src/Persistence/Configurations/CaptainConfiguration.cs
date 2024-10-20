using Crpg.Domain.Entities.Captains;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class CaptainConfiguration : IEntityTypeConfiguration<Captain>
{
    public void Configure(EntityTypeBuilder<Captain> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasMany(c => c.Formations)
               .WithOne(f => f.Captain)
               .HasForeignKey(f => f.CaptainId);
    }
}
