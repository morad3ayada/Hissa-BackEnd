using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class ParentTestResultConfiguration : BaseEntityConfiguration<ParentTestResult>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ParentTestResult> builder)
    {
        builder.ToTable("ParentTestResults");

        builder.Property(r => r.Score)
            .HasColumnType("decimal(5,2)");

        builder.Property(r => r.Feedback)
            .HasMaxLength(2000);

        builder.HasOne(r => r.ParentTest)
            .WithMany(t => t.ParentTestResults)
            .HasForeignKey(r => r.ParentTestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Student)
            .WithMany(u => u.ParentTestResults)
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.ParentTestId, r.StudentId })
            .IsUnique()
            .HasDatabaseName("IX_ParentTestResults_ParentTestId_StudentId_Unique");
    }
}
