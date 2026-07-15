using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class LiveSessionAttendanceConfiguration : BaseEntityConfiguration<LiveSessionAttendance>
{
    protected override void ConfigureEntity(EntityTypeBuilder<LiveSessionAttendance> builder)
    {
        builder.ToTable("LiveSessionAttendances");

        builder.Property(a => a.JoinedAt)
            .IsRequired();

        builder.HasOne(a => a.LiveSession)
            .WithMany(s => s.Attendances)
            .HasForeignKey(a => a.LiveSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User)
            .WithMany(u => u.LiveSessionAttendances)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.LiveSessionId)
            .HasDatabaseName("IX_LiveSessionAttendances_LiveSessionId");

        builder.HasIndex(a => a.UserId)
            .HasDatabaseName("IX_LiveSessionAttendances_UserId");
    }
}
