using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class LessonConfiguration : BaseEntityConfiguration<Lesson>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(l => l.ContentUrl)
            .HasMaxLength(500);

        builder.Property(l => l.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(l => l.IsFreePreview)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(l => l.CourseSection)
            .WithMany(s => s.Lessons)
            .HasForeignKey(l => l.CourseSectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => new { l.CourseSectionId, l.Order })
            .HasDatabaseName("IX_Lessons_CourseSectionId_Order");
    }
}
