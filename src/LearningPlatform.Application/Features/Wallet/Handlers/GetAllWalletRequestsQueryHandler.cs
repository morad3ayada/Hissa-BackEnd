using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Wallet.Common;
using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Application.Features.Wallet.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Wallet.Handlers;

public class GetAllWalletRequestsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllWalletRequestsQuery, PaginatedResponse<WalletRequestDto>>
{
    public async Task<PaginatedResponse<WalletRequestDto>> Handle(
        GetAllWalletRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<WalletRequest>().AsQueryable();

        if (request.Type is not null)
            query = query.Where(r => r.Type == request.Type.Value);

        if (request.Status is not null)
            query = query.Where(r => r.Status == request.Status.Value);

        if (request.StudentId is not null)
            query = query.Where(r => r.Wallet.StudentId == request.StudentId.Value);

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
