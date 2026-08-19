using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Wallet.DTOs;

public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public WalletTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
