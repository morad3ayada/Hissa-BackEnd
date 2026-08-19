using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Wallet.Common;

public static class WalletMapper
{
    public static WalletDto ToDto(LearningPlatform.Domain.Entities.Wallet wallet) => new()
    {
        Id = wallet.Id,
        Balance = wallet.Balance
    };

    public static WalletTransactionDto ToDto(WalletTransaction transaction) => new()
    {
        Id = transaction.Id,
        Type = transaction.Type,
        Amount = transaction.Amount,
        BalanceAfter = transaction.BalanceAfter,
        ReferenceId = transaction.ReferenceId,
        Notes = transaction.Notes,
        CreatedAt = transaction.CreatedAt
    };

    public static WalletRequestDto ToDto(WalletRequest request) => new()
    {
        Id = request.Id,
        Type = request.Type,
        Amount = request.Amount,
        Status = request.Status,
        Notes = request.Notes,
        RejectionReason = request.RejectionReason,
        CreatedAt = request.CreatedAt,
        ApprovedAt = request.ApprovedAt
    };
}
