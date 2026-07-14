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

public class ApprovePaymentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
    : IRequestHandler<ApprovePaymentCommand, ApiResponse<PaymentDto>>
{
    public async Task<ApiResponse<PaymentDto>> Handle(ApprovePaymentCommand request, CancellationToken cancellationToken)
    {
        var paymentRepository = unitOfWork.Repository<Payment>();

        var payment = await paymentRepository.AsQueryable()
            .Include(p => p.Enrollment)
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), request.PaymentId);

        if (payment.Status != PaymentStatus.Pending)
            throw new BadRequestException($"Only pending payments can be approved (current status: {payment.Status}).");

        if (payment.Enrollment.Status == EnrollmentStatus.Cancelled)
            throw new BadRequestException("Cannot approve a payment for a cancelled enrollment.");

        payment.Status = PaymentStatus.Completed;
        payment.PaidAt = DateTime.UtcNow;
        paymentRepository.Update(payment);

        var enrollmentRepository = unitOfWork.Repository<Enrollment>();
        payment.Enrollment.Status = EnrollmentStatus.Active;
        enrollmentRepository.Update(payment.Enrollment);

        await notificationService.CreateAsync(
            payment.StudentId, NotificationType.Payment, "Payment approved",
            "Your payment was approved and your course enrollment is now active.",
            cancellationToken: cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await paymentRepository.AsQueryable()
            .Include(p => p.Student)
            .Include(p => p.Enrollment).ThenInclude(e => e.Course)
            .FirstAsync(p => p.Id == payment.Id, cancellationToken);

        return ApiResponse<PaymentDto>.Success(mapper.Map<PaymentDto>(result), "Payment approved and enrollment activated.");
    }
}
