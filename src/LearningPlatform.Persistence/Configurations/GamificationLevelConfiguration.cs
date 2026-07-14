using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using LearningPlatform.Persistence.Gamification.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class GamificationLevelConfiguration : BaseEntityConfiguration<GamificationLevel>
{
    protected override void ConfigureEntity(EntityTypeBuilder<GamificationLevel> builder)
    {
        builder.ToTable("GamificationLevels");

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(l => l.LevelNumber)
            .IsUnique()
            .HasDatabaseName("IX_GamificationLevels_LevelNumber_Unique");

        builder.HasIndex(l => l.RequiredPoints)
            .IsUnique()
            .HasDatabaseName("IX_GamificationLevels_RequiredPoints_Unique");

        builder.HasData(GamificationLevelSeedData.Levels);
    }
}
