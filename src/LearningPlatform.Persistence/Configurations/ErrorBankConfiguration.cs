using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class ErrorBankConfiguration : BaseEntityConfiguration<ErrorBank>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ErrorBank> builder)
    {
        builder.ToTable("ErrorBanks");

        builder.Property(e => e.MistakeCount)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(e => e.LastMistakeAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.IsResolved)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(e => e.Student)
            .WithMany(u => u.ErrorBankEntries)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Question)
            .WithMany(q => q.ErrorBankEntries)
            .HasForeignKey(e => e.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Lesson)
            .WithMany(l => l.ErrorBankEntries)
            .HasForeignKey(e => e.LessonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.StudentAnswer)
            .WithOne(a => a.ErrorBankEntry)
            .HasForeignKey<ErrorBank>(e => e.StudentAnswerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.StudentId, e.QuestionId })
            .IsUnique()
            .HasDatabaseName("IX_ErrorBanks_StudentId_QuestionId_Unique");

        builder.HasIndex(e => e.IsResolved)
            .HasDatabaseName("IX_ErrorBanks_IsResolved");
    }
}
