using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class ParentTestQuestionConfiguration : BaseEntityConfiguration<ParentTestQuestion>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ParentTestQuestion> builder)
    {
        builder.ToTable("ParentTestQuestions");

        builder.Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(q => q.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(q => q.CorrectAnswerText)
            .HasMaxLength(1000);

        builder.HasOne(q => q.ParentTest)
            .WithMany(t => t.ParentTestQuestions)
            .HasForeignKey(q => q.ParentTestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => new { q.ParentTestId, q.Order })
            .HasDatabaseName("IX_ParentTestQuestions_ParentTestId_Order");
    }
}
