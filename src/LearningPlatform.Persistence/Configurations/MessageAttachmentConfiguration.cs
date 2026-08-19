using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class MessageAttachmentConfiguration : BaseEntityConfiguration<MessageAttachment>
{
    protected override void ConfigureEntity(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.ToTable("MessageAttachments");

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.FileUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.ContentType)
            .HasMaxLength(200);

        builder.Property(a => a.FileSize)
            .IsRequired();

        builder.Property(a => a.AttachmentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(a => a.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(a => a.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.MessageId)
            .HasDatabaseName("IX_MessageAttachments_MessageId");
    }
}
