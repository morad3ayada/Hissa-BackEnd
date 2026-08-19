using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class ConversationConfiguration : BaseEntityConfiguration<Conversation>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.Property(c => c.LastMessageAt);

        builder.HasIndex(c => c.LastMessageAt)
            .HasDatabaseName("IX_Conversations_LastMessageAt");
    }
}
