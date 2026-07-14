using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Payments.DTOs;
using LearningPlatform.Application.Features.Payments.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Payments.Handlers;

public class GetMyPaymentsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    : IRequestHandler<GetMyPaymentsQuery, PaginatedResponse<PaymentDto>>
{
    public async Task<PaginatedResponse<PaymentDto>> Handle(GetMyPaymentsQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var query = unitOfWork.Repository<Payment>()
            .AsQueryable()
            .Include(p => p.Student)
            .Include(p => p.Enrollment).ThenInclude(e => e.Course)
            .Where(p => p.StudentId == studentId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = mapper.Map<List<PaymentDto>>(items);
        var paginatedList = new PaginatedList<PaymentDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        return PaginatedResponse<PaymentDto>.Create(paginatedList);
    }
}
