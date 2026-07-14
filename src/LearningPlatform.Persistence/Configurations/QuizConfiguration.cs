using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class QuizConfiguration : BaseEntityConfiguration<Quiz>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes",
            t => t.HasCheckConstraint("CK_Quizzes_PassingScore_Range", "[PassingScore] BETWEEN 0 AND 100"));

        builder.Property(q => q.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(q => q.Scope)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(q => q.PassingScore)
            .IsRequired()
            .HasDefaultValue(60);

        builder.Property(q => q.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(q => q.IsFinalExam)
            .IsRequired()
            .HasDefaultValue(false);

        // Course-scoped quizzes are owned by the course; deleting the course removes them.
        builder.HasOne(q => q.Course)
            .WithMany(c => c.Quizzes)
            .HasForeignKey(q => q.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict (not Cascade) to avoid a second cascade path into Quiz alongside
        // Course -> CourseSection -> Lesson -> Quiz.
        builder.HasOne(q => q.Lesson)
            .WithMany(l => l.Quizzes)
            .HasForeignKey(q => q.LessonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.Challenge)
            .WithMany(c => c.Quizzes)
            .HasForeignKey(q => q.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => q.Scope)
            .HasDatabaseName("IX_Quizzes_Scope");

        builder.HasIndex(q => q.CourseId)
            .IsUnique()
            .HasFilter("[IsFinalExam] = 1")
            .HasDatabaseName("IX_Quizzes_CourseId_FinalExam_Unique");
    }
}
