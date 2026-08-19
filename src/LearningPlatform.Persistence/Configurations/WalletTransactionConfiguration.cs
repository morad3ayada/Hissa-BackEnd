using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class WalletTransactionConfiguration : BaseEntityConfiguration<WalletTransaction>
{
    protected override void ConfigureEntity(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.ToTable("WalletTransactions");

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.BalanceAfter)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Notes)
            .HasMaxLength(1000);

        builder.HasOne(t => t.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.WalletId)
            .HasDatabaseName("IX_WalletTransactions_WalletId");

        builder.HasIndex(t => t.Type)
            .HasDatabaseName("IX_WalletTransactions_Type");
    }
}
