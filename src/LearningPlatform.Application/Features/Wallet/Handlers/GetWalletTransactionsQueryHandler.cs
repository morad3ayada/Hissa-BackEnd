using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Wallet.Common;
using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Application.Features.Wallet.Interfaces;
using LearningPlatform.Application.Features.Wallet.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Wallet.Handlers;

public class GetWalletTransactionsQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IWalletService walletService)
    : IRequestHandler<GetWalletTransactionsQuery, PaginatedResponse<WalletTransactionDto>>
{
    public async Task<PaginatedResponse<WalletTransactionDto>> Handle(
        GetWalletTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var wallet = await walletService.GetOrCreateAsync(currentUser.UserId!.Value, cancellationToken);

        var query = unitOfWork.Repository<WalletTransaction>()
            .AsQueryable()
            .Where(t => t.WalletId == wallet.Id);

        if (request.Type is not null)
            query = query.Where(t => t.Type == request.Type.Value);

        query = query.OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var paginated = new PaginatedList<WalletTransactionDto>(
            items.Select(WalletMapper.ToDto).ToList(),
            totalCount,
            request.PageNumber,
            request.PageSize);

        return PaginatedResponse<WalletTransactionDto>.Create(paginated);
    }
}
