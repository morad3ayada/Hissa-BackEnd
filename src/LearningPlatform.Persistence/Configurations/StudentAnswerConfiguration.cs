using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class StudentAnswerConfiguration : BaseEntityConfiguration<StudentAnswer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<StudentAnswer> builder)
    {
        builder.ToTable("StudentAnswers");

        builder.Property(a => a.TextResponse)
            .HasMaxLength(2000);

        builder.Property(a => a.IsCorrect)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.AnsweredAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(a => a.QuizResult)
            .WithMany(r => r.StudentAnswers)
            .HasForeignKey(a => a.QuizResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Question)
            .WithMany(q => q.StudentAnswers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.SelectedAnswer)
            .WithMany(o => o.StudentAnswers)
            .HasForeignKey(a => a.SelectedAnswerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Student)
            .WithMany(u => u.StudentAnswers)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.QuizResultId, a.QuestionId })
            .IsUnique()
            .HasDatabaseName("IX_StudentAnswers_QuizResultId_QuestionId_Unique");
    }
}
