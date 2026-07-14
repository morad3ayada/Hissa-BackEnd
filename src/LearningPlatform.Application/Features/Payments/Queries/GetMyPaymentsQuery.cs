using LearningPlatform.Application.Features.Payments.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Payments.Queries;

public record GetMyPaymentsQuery : IRequest<PaginatedResponse<PaymentDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
