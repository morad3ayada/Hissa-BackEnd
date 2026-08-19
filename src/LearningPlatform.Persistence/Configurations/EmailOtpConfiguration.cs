using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class EmailOtpConfiguration : BaseEntityConfiguration<EmailOtp>
{
    protected override void ConfigureEntity(EntityTypeBuilder<EmailOtp> builder)
    {
        builder.ToTable("EmailOtps");

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.OtpHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.Attempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.MaxAttempts)
            .IsRequired()
            .HasDefaultValue(5);

        builder.HasIndex(e => e.Email)
            .HasDatabaseName("IX_EmailOtps_Email");

        builder.HasIndex(e => new { e.Email, e.IsUsed })
            .HasDatabaseName("IX_EmailOtps_Email_IsUsed");
    }
}
