using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using LearningPlatform.Persistence.Gamification.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class AvatarItemConfiguration : BaseEntityConfiguration<AvatarItem>
{
    protected override void ConfigureEntity(EntityTypeBuilder<AvatarItem> builder)
    {
        builder.ToTable("AvatarItems");

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.PriceInPoints)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(a => a.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(a => a.Category)
            .HasDatabaseName("IX_AvatarItems_Category");

        builder.HasData(AvatarItemSeedData.Items);
    }
}
