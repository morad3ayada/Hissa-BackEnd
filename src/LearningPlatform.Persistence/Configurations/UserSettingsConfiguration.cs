using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class UserSettingsConfiguration : BaseEntityConfiguration<UserSettings>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserSettings> builder)
    {
        builder.ToTable("UserSettings");

        builder.Property(s => s.Language)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("ar");

        builder.Property(s => s.Theme)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Light");

        builder.Property(s => s.EmailNotificationsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.PushNotificationsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(s => s.User)
            .WithOne(u => u.Settings)
            .HasForeignKey<UserSettings>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.UserId)
            .IsUnique()
            .HasDatabaseName("IX_UserSettings_UserId_Unique");
    }
}
