using LearningPlatform.Application.Features.Payments.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Payments.Queries;

public record GetPaymentDetailsQuery(Guid Id) : IRequest<ApiResponse<PaymentDto>>;
