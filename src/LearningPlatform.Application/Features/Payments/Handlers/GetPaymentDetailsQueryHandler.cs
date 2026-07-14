using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Payments.DTOs;
using LearningPlatform.Application.Features.Payments.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Payments.Handlers;

public class GetPaymentDetailsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    : IRequestHandler<GetPaymentDetailsQuery, ApiResponse<PaymentDto>>
{
    public async Task<ApiResponse<PaymentDto>> Handle(GetPaymentDetailsQuery request, CancellationToken cancellationToken)
    {
        var payment = await unitOfWork.Repository<Payment>()
            .AsQueryable()
            .Include(p => p.Student)
            .Include(p => p.Enrollment).ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), request.Id);

        var isOwner = currentUser.UserId == payment.StudentId;
        var isAdmin = currentUser.IsInRole(nameof(UserRole.Admin));

        if (!isOwner && !isAdmin)
            throw new ForbiddenException("You do not have permission to view this payment.");

        return ApiResponse<PaymentDto>.Success(mapper.Map<PaymentDto>(payment));
    }
}
