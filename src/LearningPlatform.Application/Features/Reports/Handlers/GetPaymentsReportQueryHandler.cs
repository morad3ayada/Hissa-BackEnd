using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Dashboard.DTOs;
using LearningPlatform.Application.Features.Reports.DTOs;
using LearningPlatform.Application.Features.Reports.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Reports.Handlers;

public class GetPaymentsReportQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPaymentsReportQuery, ApiResponse<PaymentsReportDto>>
{
    public async Task<ApiResponse<PaymentsReportDto>> Handle(GetPaymentsReportQuery request, CancellationToken cancellationToken)
    {
        var byStatus = await unitOfWork.Repository<Payment>().AsQueryable()
            .GroupBy(p => p.Status)
            .Select(g => new PaymentStatusBreakdownDto
            {
                Status = g.Key.ToString(),
                Count = g.Count(),
                TotalAmount = g.Sum(p => p.Amount)
            })
            .ToListAsync(cancellationToken);

        var byMethod = await unitOfWork.Repository<Payment>().AsQueryable()
            .GroupBy(p => p.PaymentMethod)
            .Select(g => new PaymentMethodBreakdownDto
            {
                Method = g.Key,
                Count = g.Count(),
                TotalAmount = g.Sum(p => p.Amount)
            })
            .ToListAsync(cancellationToken);

        var totalRevenue = await unitOfWork.Repository<Payment>().AsQueryable()
            .Where(p => p.Status == PaymentStatus.Completed)
            .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0;

        var dto = new PaymentsReportDto
        {
            ByStatus = byStatus,
            ByMethod = byMethod,
            TotalRevenue = totalRevenue
        };

        return ApiResponse<PaymentsReportDto>.Success(dto);
    }
}
