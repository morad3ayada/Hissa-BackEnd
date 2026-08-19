using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class ChatMessageConfiguration : BaseEntityConfiguration<ChatMessage>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");

        builder.Property(m => m.Content)
            .HasMaxLength(5000);

        builder.Property(m => m.MessageType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.IsEdited)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.IsDeletedForSender)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.IsDeletedForRecipient)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.DeletedForEveryoneAt);

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Sender)
            .WithMany(u => u.SentChatMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.ReplyToMessage)
            .WithMany()
            .HasForeignKey(m => m.ReplyToMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.ConversationId, m.CreatedAt })
            .HasDatabaseName("IX_ChatMessages_ConversationId_CreatedAt");

        builder.HasIndex(m => new { m.ConversationId, m.Status })
            .HasDatabaseName("IX_ChatMessages_ConversationId_Status");
    }
}
