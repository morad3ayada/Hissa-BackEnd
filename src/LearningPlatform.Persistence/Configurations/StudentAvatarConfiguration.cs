using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class StudentAvatarConfiguration : BaseEntityConfiguration<StudentAvatar>
{
    protected override void ConfigureEntity(EntityTypeBuilder<StudentAvatar> builder)
    {
        builder.ToTable("StudentAvatars");

        builder.Property(sa => sa.IsEquipped)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(sa => sa.AcquiredAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(sa => sa.Student)
            .WithMany(u => u.StudentAvatars)
            .HasForeignKey(sa => sa.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sa => sa.AvatarItem)
            .WithMany(a => a.StudentAvatars)
            .HasForeignKey(sa => sa.AvatarItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(sa => new { sa.StudentId, sa.AvatarItemId })
            .IsUnique()
            .HasDatabaseName("IX_StudentAvatars_StudentId_AvatarItemId_Unique");
    }
}
