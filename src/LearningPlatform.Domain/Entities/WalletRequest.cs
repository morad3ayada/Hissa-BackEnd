using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

/// <summary>Student-initiated deposit or withdrawal request that an admin reviews.
/// Only an Approved request ever moves the wallet balance.</summary>
public class WalletRequest : BaseEntity
{
    public Guid WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;

    public WalletRequestType Type { get; set; }
    public decimal Amount { get; set; }
    public WalletRequestStatus Status { get; set; } = WalletRequestStatus.Pending;
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedById { get; set; }
}
