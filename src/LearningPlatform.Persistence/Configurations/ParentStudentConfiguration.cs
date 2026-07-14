using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class ParentStudentConfiguration : BaseEntityConfiguration<ParentStudent>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ParentStudent> builder)
    {
        builder.ToTable("ParentStudents");

        builder.Property(ps => ps.RelationshipType)
            .HasMaxLength(50);

        builder.Property(ps => ps.LinkedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(ps => ps.Parent)
            .WithMany(u => u.ChildLinks)
            .HasForeignKey(ps => ps.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.Student)
            .WithMany(u => u.ParentLinks)
            .HasForeignKey(ps => ps.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ps => new { ps.ParentId, ps.StudentId })
            .IsUnique()
            .HasDatabaseName("IX_ParentStudents_ParentId_StudentId_Unique");
    }
}
