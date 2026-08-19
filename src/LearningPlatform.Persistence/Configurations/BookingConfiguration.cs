using LearningPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Subject)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(b => b.Date)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(b => b.StartTime)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(b => b.EndTime)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(b => b.Notes)
            .HasMaxLength(2000);

        builder.Property(b => b.CancellationReason)
            .HasMaxLength(1000);

        builder.HasOne(b => b.Teacher)
            .WithMany(u => u.TeacherBookings)
            .HasForeignKey(b => b.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Student)
            .WithMany(u => u.StudentBookings)
            .HasForeignKey(b => b.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.TeacherId, b.Date, b.StartTime });
        builder.HasIndex(b => new { b.StudentId, b.Date });
        builder.HasIndex(b => b.Status);
    }
}
