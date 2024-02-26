using Crpg.Domain.Entities.Captains;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class CaptainFormationConfiguration : IEntityTypeConfiguration<CaptainFormation>
{
    public void Configure(EntityTypeBuilder<CaptainFormation> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Weight)
            .IsRequired();
        builder.HasOne(f => f.Character)
            .WithMany()
            .HasForeignKey(f => f.CharacterId);
    }
}
