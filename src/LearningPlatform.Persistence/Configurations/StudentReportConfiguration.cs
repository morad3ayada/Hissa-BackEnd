using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class StudentReportConfiguration : BaseEntityConfiguration<StudentReport>
{
    protected override void ConfigureEntity(EntityTypeBuilder<StudentReport> builder)
    {
        builder.ToTable("StudentReports");

        builder.Property(r => r.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Content)
            .IsRequired();

        builder.HasOne(r => r.Instructor)
            .WithMany()
            .HasForeignKey(r => r.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Student)
            .WithMany()
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.StudentId);
        builder.HasIndex(r => r.InstructorId);
    }
}
