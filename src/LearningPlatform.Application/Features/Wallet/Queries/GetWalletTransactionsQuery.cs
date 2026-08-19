using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Wallet.Queries;

public record GetWalletTransactionsQuery : IRequest<PaginatedResponse<WalletTransactionDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public WalletTransactionType? Type { get; init; }
}
