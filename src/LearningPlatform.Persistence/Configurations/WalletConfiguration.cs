using LearningPlatform.Domain.Entities;
using LearningPlatform.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Persistence.Configurations;

public class WalletConfiguration : BaseEntityConfiguration<Wallet>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.Property(w => w.Balance)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(w => w.Student)
            .WithOne(u => u.Wallet)
            .HasForeignKey<Wallet>(w => w.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => w.StudentId)
            .IsUnique()
            .HasDatabaseName("IX_Wallets_StudentId_Unique");
    }
}
