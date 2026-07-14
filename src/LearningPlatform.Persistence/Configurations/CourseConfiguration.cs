using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class CourseConfiguration : BaseEntityConfiguration<Course>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(c => c.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(c => c.Category)
            .HasMaxLength(100);

        builder.Property(c => c.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);

        builder.Property(c => c.DiscountPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(CourseStatus.Draft);

        builder.Property(c => c.Level)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(DifficultyLevel.Beginner);

        builder.Property(c => c.Language)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("ar");

        builder.HasOne(c => c.Instructor)
            .WithMany(u => u.InstructorCourses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Courses_Slug_Unique");

        builder.HasIndex(c => c.Title)
            .HasDatabaseName("IX_Courses_Title");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_Courses_Status");
    }
}
