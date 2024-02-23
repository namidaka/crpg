using Crpg.Domain.Entities.Captains;
using Crpg.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class CaptainConfiguration : IEntityTypeConfiguration<Captain>
{
    public void Configure(EntityTypeBuilder<Captain> builder)
    {
        builder.HasOne<User>(c => c.User!)
            .WithOne(u => u.Captain!)
        .HasForeignKey<Captain>(u => u.UserId);

        builder.HasKey(c => c.Id);
    }
}
