using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class CourseSectionConfiguration : BaseEntityConfiguration<CourseSection>
{
    protected override void ConfigureEntity(EntityTypeBuilder<CourseSection> builder)
    {
        builder.ToTable("CourseSections");

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(s => s.Course)
            .WithMany(c => c.CourseSections)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.CourseId, s.Order })
            .HasDatabaseName("IX_CourseSections_CourseId_Order");
    }
}
