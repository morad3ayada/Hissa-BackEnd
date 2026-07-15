using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Payments.DTOs;
using LearningPlatform.Application.Features.Payments.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Payments.Handlers;

public class GetPendingPaymentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetPendingPaymentsQuery, PaginatedResponse<PaymentDto>>
{
    public async Task<PaginatedResponse<PaymentDto>> Handle(GetPendingPaymentsQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<Payment>()
            .AsQueryable()
            .Include(p => p.Student)
            .Include(p => p.Enrollment).ThenInclude(e => e.Course)
            .Where(p => p.Status == PaymentStatus.Pending);

        if (request.InstructorId.HasValue)
        {
            query = query.Where(p => p.Enrollment.Course.InstructorId == request.InstructorId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = mapper.Map<List<PaymentDto>>(items);
        var paginatedList = new PaginatedList<PaymentDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        return PaginatedResponse<PaymentDto>.Create(paginatedList);
    }
}
