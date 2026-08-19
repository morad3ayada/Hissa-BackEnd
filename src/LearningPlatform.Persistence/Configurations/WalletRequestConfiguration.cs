using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class WalletRequestConfiguration : BaseEntityConfiguration<WalletRequest>
{
    protected override void ConfigureEntity(EntityTypeBuilder<WalletRequest> builder)
    {
        builder.ToTable("WalletRequests");

        builder.Property(r => r.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(WalletRequestStatus.Pending);

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.Property(r => r.RejectionReason)
            .HasMaxLength(1000);

        builder.HasOne(r => r.Wallet)
            .WithMany(w => w.Requests)
            .HasForeignKey(r => r.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.WalletId, r.Status })
            .HasDatabaseName("IX_WalletRequests_WalletId_Status");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_WalletRequests_Status");
    }
}
