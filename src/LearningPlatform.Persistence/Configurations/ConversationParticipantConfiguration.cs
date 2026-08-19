using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class ConversationParticipantConfiguration : BaseEntityConfiguration<ConversationParticipant>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.ToTable("ConversationParticipants");

        builder.Property(p => p.IsMuted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsHidden)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.HiddenAt);

        builder.Property(p => p.LastReadMessageId);

        builder.HasOne(p => p.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(p => p.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.User)
            .WithMany(u => u.ConversationParticipations)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.UserId, p.ConversationId })
            .IsUnique()
            .HasDatabaseName("IX_ConversationParticipants_UserId_ConversationId");

        builder.HasIndex(p => new { p.ConversationId, p.IsHidden })
            .HasDatabaseName("IX_ConversationParticipants_ConversationId_IsHidden");
    }
}
