using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class TeacherProfileConfiguration : BaseEntityConfiguration<TeacherProfile>
{
    protected override void ConfigureEntity(EntityTypeBuilder<TeacherProfile> builder)
    {
        builder.ToTable("TeacherProfiles");

        builder.Property(t => t.RealName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Specialization)
            .HasMaxLength(200);

        builder.Property(t => t.Governorate)
            .HasMaxLength(100);

        builder.Property(t => t.Bio)
            .HasMaxLength(2000);

        builder.Property(t => t.LessonPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(t => t.Subjects)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.Grades)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.Certificates)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.Qualifications)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.RequiredDocuments)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        builder.HasOne(t => t.User)
            .WithOne()
            .HasForeignKey<TeacherProfile>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.UserId)
            .IsUnique()
            .HasDatabaseName("IX_TeacherProfiles_UserId_Unique");

        builder.Property(t => t.VerificationStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(TeacherVerificationStatus.UnderReview);

        builder.Property(t => t.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(t => t.AcceptingBookings)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
