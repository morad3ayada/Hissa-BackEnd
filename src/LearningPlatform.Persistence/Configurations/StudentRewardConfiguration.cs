using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class StudentRewardConfiguration : BaseEntityConfiguration<StudentReward>
{
    protected override void ConfigureEntity(EntityTypeBuilder<StudentReward> builder)
    {
        builder.ToTable("StudentRewards");

        builder.Property(sr => sr.EarnedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(sr => sr.Student)
            .WithMany(u => u.StudentRewards)
            .HasForeignKey(sr => sr.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.Reward)
            .WithMany(r => r.StudentRewards)
            .HasForeignKey(sr => sr.RewardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.SourceChallenge)
            .WithMany()
            .HasForeignKey(sr => sr.SourceChallengeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(sr => new { sr.StudentId, sr.RewardId, sr.SourceChallengeId })
            .HasDatabaseName("IX_StudentRewards_StudentId_RewardId_SourceChallengeId");
    }
}
