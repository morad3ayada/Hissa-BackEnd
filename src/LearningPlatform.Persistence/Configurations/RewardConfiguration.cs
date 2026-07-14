using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using LearningPlatform.Persistence.Gamification.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class RewardConfiguration : BaseEntityConfiguration<Reward>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Reward> builder)
    {
        builder.ToTable("Rewards");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.PointsValue)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(r => r.IconUrl)
            .HasMaxLength(500);

        builder.Property(r => r.TriggerType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(r => r.AvatarItem)
            .WithMany(a => a.Rewards)
            .HasForeignKey(r => r.AvatarItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.TriggerType)
            .IsUnique()
            .HasFilter("[TriggerType] IS NOT NULL")
            .HasDatabaseName("IX_Rewards_TriggerType_Unique");

        builder.HasData(AchievementRewardSeedData.Rewards);
    }
}
