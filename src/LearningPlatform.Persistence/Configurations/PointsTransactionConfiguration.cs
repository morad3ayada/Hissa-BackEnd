using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class PointsTransactionConfiguration : BaseEntityConfiguration<PointsTransaction>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PointsTransaction> builder)
    {
        builder.ToTable("PointsTransactions");

        builder.Property(t => t.Points)
            .IsRequired();

        builder.Property(t => t.Reason)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(t => t.Notes)
            .HasMaxLength(500);

        builder.HasOne(t => t.Student)
            .WithMany(u => u.PointsTransactions)
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Idempotency guard: the same student can only be awarded points once per
        // (Reason, SourceId) pair — e.g. can't earn "LessonCompleted" twice for the same lesson.
        builder.HasIndex(t => new { t.StudentId, t.Reason, t.SourceId })
            .IsUnique()
            .HasFilter("[SourceId] IS NOT NULL")
            .HasDatabaseName("IX_PointsTransactions_StudentId_Reason_SourceId_Unique");
    }
}
