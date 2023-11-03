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

        builder.HasOne(e => e.User)
            .WithOne(e => e.ClanMembership)
            .HasForeignKey<ClanMember>(e => e.UserId)
            .IsRequired();

        builder.HasOne(e => e.Clan)
            .WithMany(e => e.Members)
            .HasForeignKey(e => e.ClanId)
            .IsRequired();
    }
}
