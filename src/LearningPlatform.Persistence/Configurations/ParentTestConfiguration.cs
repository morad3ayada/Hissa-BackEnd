using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class ParentTestConfiguration : BaseEntityConfiguration<ParentTest>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ParentTest> builder)
    {
        builder.ToTable("ParentTests");

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Instructions)
            .HasMaxLength(2000);

        builder.Property(t => t.AssignedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(t => t.Parent)
            .WithMany(u => u.ParentTestsAssigned)
            .HasForeignKey(t => t.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Student)
            .WithMany(u => u.ParentTestsReceived)
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.StudentId, t.DueDate })
            .HasDatabaseName("IX_ParentTests_StudentId_DueDate");
    }
}
