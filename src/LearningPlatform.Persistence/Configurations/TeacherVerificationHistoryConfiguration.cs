using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class TeacherVerificationHistoryConfiguration : BaseEntityConfiguration<TeacherVerificationHistory>
{
    protected override void ConfigureEntity(EntityTypeBuilder<TeacherVerificationHistory> builder)
    {
        builder.ToTable("TeacherVerificationHistory");

        builder.Property(h => h.Reason)
            .HasMaxLength(1000);

        builder.Property(h => h.OldStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(h => h.NewStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(h => h.TeacherProfile)
            .WithMany(t => t.VerificationHistory)
            .HasForeignKey(h => h.TeacherProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(h => h.TeacherProfileId)
            .HasDatabaseName("IX_TeacherVerificationHistory_TeacherProfileId");
    }
}
