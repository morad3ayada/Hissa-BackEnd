using LearningPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class TeacherAvailabilityConfiguration : IEntityTypeConfiguration<TeacherAvailability>
{
    public void Configure(EntityTypeBuilder<TeacherAvailability> builder)
    {
        builder.ToTable("TeacherAvailabilities");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.DayOfWeek)
            .IsRequired();

        builder.Property(a => a.StartTime)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(a => a.EndTime)
            .HasColumnType("time")
            .IsRequired();

        builder.HasOne(a => a.Teacher)
            .WithMany(u => u.TeacherAvailabilities)
            .HasForeignKey(a => a.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.TeacherId, a.DayOfWeek });
    }
}
