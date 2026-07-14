using LearningPlatform.Persistence.Identity.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Identity.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.ToTable("Roles");

        builder.Property(r => r.Name).HasMaxLength(50);
        builder.Property(r => r.NormalizedName).HasMaxLength(50);

        builder.HasData(RoleSeedData.Roles);
    }
}
