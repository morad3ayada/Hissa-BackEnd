using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class AnswerConfiguration : BaseEntityConfiguration<Answer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers");

        builder.Property(a => a.Text)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.IsCorrect)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.QuestionId, a.Order })
            .HasDatabaseName("IX_Answers_QuestionId_Order");
    }
}
