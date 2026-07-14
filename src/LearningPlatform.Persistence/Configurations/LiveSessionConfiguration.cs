using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class LiveSessionConfiguration : BaseEntityConfiguration<LiveSession>
{
    protected override void ConfigureEntity(EntityTypeBuilder<LiveSession> builder)
    {
        builder.ToTable("LiveSessions");

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(2000);

        builder.Property(s => s.MeetingPlatform)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.MeetingLink)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(s => s.MeetingPassword)
            .HasMaxLength(100);

        builder.Property(s => s.StartDateTime)
            .IsRequired();

        builder.Property(s => s.EndDateTime)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.LiveSessionStatus.Scheduled);

        builder.HasOne(s => s.Course)
            .WithMany(c => c.LiveSessions)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Instructor)
            .WithMany(u => u.LiveSessionsHosted)
            .HasForeignKey(s => s.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.InstructorId, s.StartDateTime })
            .HasDatabaseName("IX_LiveSessions_InstructorId_StartDateTime");

        builder.HasIndex(s => new { s.CourseId, s.StartDateTime })
            .HasDatabaseName("IX_LiveSessions_CourseId_StartDateTime");
    }
}
