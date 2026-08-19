using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Wallet.Queries;

public record GetMyWalletRequestsQuery : IRequest<PaginatedResponse<WalletRequestDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public WalletRequestType? Type { get; init; }
    public WalletRequestStatus? Status { get; init; }
}
