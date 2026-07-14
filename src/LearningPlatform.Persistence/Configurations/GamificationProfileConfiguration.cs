using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class GamificationProfileConfiguration : BaseEntityConfiguration<GamificationProfile>
{
    protected override void ConfigureEntity(EntityTypeBuilder<GamificationProfile> builder)
    {
        builder.ToTable("GamificationProfiles");

        builder.Property(p => p.TotalPoints)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.CurrentLevel)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(p => p.AvatarGender)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10)
            .HasDefaultValue(Domain.Enums.AvatarGender.Boy);

        builder.HasOne(p => p.Student)
            .WithOne(u => u.GamificationProfile)
            .HasForeignKey<GamificationProfile>(p => p.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.StudentId)
            .IsUnique()
            .HasDatabaseName("IX_GamificationProfiles_StudentId_Unique");
    }
}
