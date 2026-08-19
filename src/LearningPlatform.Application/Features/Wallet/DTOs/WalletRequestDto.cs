using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Wallet.DTOs;

public class WalletRequestDto
{
    public Guid Id { get; set; }
    public WalletRequestType Type { get; set; }
    public decimal Amount { get; set; }
    public WalletRequestStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
