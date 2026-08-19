using LearningPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class TeacherStudentNoteConfiguration : IEntityTypeConfiguration<TeacherStudentNote>
{
    public void Configure(EntityTypeBuilder<TeacherStudentNote> builder)
    {
        builder.ToTable("TeacherStudentNotes");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Note)
            .HasMaxLength(4000)
            .IsRequired();

        builder.HasOne(n => n.Teacher)
            .WithMany(u => u.TeacherNotes)
            .HasForeignKey(n => n.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Student)
            .WithMany(u => u.StudentNotes)
            .HasForeignKey(n => n.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(n => new { n.TeacherId, n.StudentId });
    }
}
