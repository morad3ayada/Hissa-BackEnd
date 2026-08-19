using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class BlockedUserConfiguration : BaseEntityConfiguration<BlockedUser>
{
    protected override void ConfigureEntity(EntityTypeBuilder<BlockedUser> builder)
    {
        builder.ToTable("BlockedUsers");

        builder.HasOne(b => b.User)
            .WithMany(u => u.BlockedUsers)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.BlockedUserInfo)
            .WithMany(u => u.BlockedByUsers)
            .HasForeignKey(b => b.BlockedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.UserId, b.BlockedUserId })
            .IsUnique()
            .HasDatabaseName("IX_BlockedUsers_UserId_BlockedUserId");
    }
}
