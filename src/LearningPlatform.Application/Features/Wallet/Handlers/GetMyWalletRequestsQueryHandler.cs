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

public class GetMyWalletRequestsQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IWalletService walletService)
    : IRequestHandler<GetMyWalletRequestsQuery, PaginatedResponse<WalletRequestDto>>
{
    public async Task<PaginatedResponse<WalletRequestDto>> Handle(
        GetMyWalletRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var wallet = await walletService.GetOrCreateAsync(currentUser.UserId!.Value, cancellationToken);

        var query = unitOfWork.Repository<WalletRequest>()
            .AsQueryable()
            .Where(r => r.WalletId == wallet.Id);

        if (request.Type is not null)
            query = query.Where(r => r.Type == request.Type.Value);

        if (request.Status is not null)
            query = query.Where(r => r.Status == request.Status.Value);

        query = query.OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var paginated = new PaginatedList<WalletRequestDto>(
            items.Select(WalletMapper.ToDto).ToList(),
            totalCount,
            request.PageNumber,
            request.PageSize);

        return PaginatedResponse<WalletRequestDto>.Create(paginated);
    }
}
