using LearningPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class TeacherUnavailableSlotConfiguration : IEntityTypeConfiguration<TeacherUnavailableSlot>
{
    public void Configure(EntityTypeBuilder<TeacherUnavailableSlot> builder)
    {
        builder.ToTable("TeacherUnavailableSlots");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Date)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(s => s.StartTime)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(s => s.EndTime)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(s => s.Reason)
            .HasMaxLength(500);

        builder.HasOne(s => s.Teacher)
            .WithMany(u => u.TeacherUnavailableSlots)
            .HasForeignKey(s => s.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.TeacherId, s.Date });
    }
}
