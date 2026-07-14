using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class CourseProgressConfiguration : BaseEntityConfiguration<CourseProgress>
{
    protected override void ConfigureEntity(EntityTypeBuilder<CourseProgress> builder)
    {
        builder.ToTable("CourseProgresses",
            t => t.HasCheckConstraint("CK_CourseProgresses_WatchPercentage_Range", "[WatchPercentage] BETWEEN 0 AND 100"));

        builder.Property(p => p.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.CurrentSecond)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.WatchPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0m);

        builder.HasOne(p => p.Enrollment)
            .WithMany(e => e.CourseProgresses)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Lesson)
            .WithMany(l => l.CourseProgresses)
            .HasForeignKey(p => p.LessonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Student)
            .WithMany(u => u.CourseProgresses)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.EnrollmentId, p.LessonId })
            .IsUnique()
            .HasDatabaseName("IX_CourseProgresses_EnrollmentId_LessonId_Unique");

        // The unique index above is EnrollmentId-first, so it doesn't serve the
        // per-student lookups Dashboard/Reports run directly against StudentId
        // (last activity, study hours, "last lesson watched").
        builder.HasIndex(p => p.StudentId)
            .HasDatabaseName("IX_CourseProgresses_StudentId");
    }
}
