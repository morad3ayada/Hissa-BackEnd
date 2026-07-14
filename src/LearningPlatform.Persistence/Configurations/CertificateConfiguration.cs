using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class CertificateConfiguration : BaseEntityConfiguration<Certificate>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");

        builder.Property(c => c.CertificateNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.IssuedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.CertificateUrl)
            .HasMaxLength(500);

        builder.HasOne(c => c.Student)
            .WithMany(u => u.Certificates)
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Course)
            .WithMany(co => co.Certificates)
            .HasForeignKey(c => c.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Enrollment)
            .WithOne(e => e.Certificate)
            .HasForeignKey<Certificate>(c => c.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CertificateNumber)
            .IsUnique()
            .HasDatabaseName("IX_Certificates_CertificateNumber_Unique");

        builder.HasIndex(c => new { c.StudentId, c.CourseId })
            .IsUnique()
            .HasDatabaseName("IX_Certificates_StudentId_CourseId_Unique");
    }
}
