using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Payments.Commands;
using LearningPlatform.Application.Features.Payments.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Payments.Handlers;

public class RejectPaymentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
    : IRequestHandler<RejectPaymentCommand, ApiResponse<PaymentDto>>
{
    public async Task<ApiResponse<PaymentDto>> Handle(RejectPaymentCommand request, CancellationToken cancellationToken)
    {
        var paymentRepository = unitOfWork.Repository<Payment>();

        var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), request.PaymentId);

        if (payment.Status != PaymentStatus.Pending)
            throw new BadRequestException($"Only pending payments can be rejected (current status: {payment.Status}).");

        payment.Status = PaymentStatus.Failed;
        payment.RejectionReason = request.RejectionReason;
        paymentRepository.Update(payment);

        await notificationService.CreateAsync(
            payment.StudentId, NotificationType.Payment, "Payment rejected",
            $"Your payment was rejected. Reason: {request.RejectionReason}",
            cancellationToken: cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await paymentRepository.AsQueryable()
            .Include(p => p.Student)
            .Include(p => p.Enrollment).ThenInclude(e => e.Course)
            .FirstAsync(p => p.Id == payment.Id, cancellationToken);

        return ApiResponse<PaymentDto>.Success(mapper.Map<PaymentDto>(result), "Payment rejected.");
    }
}
