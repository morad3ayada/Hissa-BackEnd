using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

/// <summary>One-to-one money balance owned by a student. Balance only ever changes
/// through approved wallet requests, each recorded as a WalletTransaction.</summary>
public class Wallet : BaseEntity
{
    public decimal Balance { get; set; }

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public ICollection<WalletTransaction> Transactions { get; set; } = [];
    public ICollection<WalletRequest> Requests { get; set; } = [];
}
