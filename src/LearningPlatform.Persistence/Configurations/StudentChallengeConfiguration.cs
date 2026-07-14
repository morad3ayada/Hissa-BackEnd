using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class StudentChallengeConfiguration : BaseEntityConfiguration<StudentChallenge>
{
    protected override void ConfigureEntity(EntityTypeBuilder<StudentChallenge> builder)
    {
        builder.ToTable("StudentChallenges");

        builder.Property(sc => sc.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(ChallengeStatus.NotStarted);

        builder.Property(sc => sc.Progress)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(sc => sc.Score)
            .HasColumnType("decimal(5,2)");

        builder.HasOne(sc => sc.Student)
            .WithMany(u => u.StudentChallenges)
            .HasForeignKey(sc => sc.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sc => sc.Challenge)
            .WithMany(c => c.StudentChallenges)
            .HasForeignKey(sc => sc.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(sc => new { sc.StudentId, sc.ChallengeId })
            .IsUnique()
            .HasDatabaseName("IX_StudentChallenges_StudentId_ChallengeId_Unique");
    }
}
