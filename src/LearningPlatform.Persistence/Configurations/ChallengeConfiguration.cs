using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class ChallengeConfiguration : BaseEntityConfiguration<Challenge>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Challenge> builder)
    {
        builder.ToTable("Challenges");

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.PointsReward)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.DurationInMinutes)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.ChallengeStatus.NotStarted);

        builder.HasOne(c => c.Reward)
            .WithMany(r => r.Challenges)
            .HasForeignKey(c => c.RewardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Challenger)
            .WithMany()
            .HasForeignKey(c => c.ChallengerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Opponent)
            .WithMany()
            .HasForeignKey(c => c.OpponentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Winner)
            .WithMany()
            .HasForeignKey(c => c.WinnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Quiz)
            .WithMany()
            .HasForeignKey(c => c.QuizId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("IX_Challenges_IsActive");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_Challenges_Status");
    }
}
