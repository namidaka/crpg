using Crpg.Domain.Entities.Clans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crpg.Persistence.Configurations;

public class ClanMemberConfiguration : IEntityTypeConfiguration<ClanMember>
{
    public void Configure(EntityTypeBuilder<ClanMember> builder)
    {
        builder.HasKey(cm => cm.UserId);
        builder.HasQueryFilter(cm => cm.User!.DeletedAt == null);

        builder.HasOne(cm => cm.User)
            .WithOne(u => u.ClanMembership)
            .HasForeignKey<ClanMember>(cm => cm.UserId)
            .IsRequired();

        builder.HasOne(cm => cm.Clan)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.ClanId)
            .IsRequired();
    }
}
