using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class QuizResultConfiguration : BaseEntityConfiguration<QuizResult>
{
    protected override void ConfigureEntity(EntityTypeBuilder<QuizResult> builder)
    {
        builder.ToTable("QuizResults");

        builder.Property(r => r.Score)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(r => r.AttemptNumber)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(r => r.StartedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(r => r.Quiz)
            .WithMany(q => q.QuizResults)
            .HasForeignKey(r => r.QuizId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Student)
            .WithMany(u => u.QuizResults)
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.StudentId, r.QuizId, r.AttemptNumber })
            .IsUnique()
            .HasDatabaseName("IX_QuizResults_StudentId_QuizId_AttemptNumber_Unique");

        // The unique index above is StudentId-first, so it doesn't serve per-quiz aggregate
        // queries (Dashboard/Reports group and filter by QuizId across all students).
        builder.HasIndex(r => r.QuizId)
            .HasDatabaseName("IX_QuizResults_QuizId");
    }
}
