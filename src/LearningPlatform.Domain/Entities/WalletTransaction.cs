using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

/// <summary>Ledger entry recording an actual movement of funds into/out of a wallet.
/// Only created when a wallet request is approved; never mutated afterwards.</summary>
public class WalletTransaction : BaseEntity
{
    public Guid WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;

    public WalletTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? Notes { get; set; }
}
