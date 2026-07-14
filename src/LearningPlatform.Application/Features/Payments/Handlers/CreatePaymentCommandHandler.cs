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

public class CreatePaymentCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IFileStorageService fileStorageService,
    IMapper mapper)
    : IRequestHandler<CreatePaymentCommand, ApiResponse<PaymentDto>>
{
    public async Task<ApiResponse<PaymentDto>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var enrollment = await unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.CourseId == request.CourseId && e.StudentId == studentId, cancellationToken)
            ?? throw new NotFoundException("Enroll in this course before submitting a payment.");

        if (enrollment.Status == EnrollmentStatus.Active || enrollment.Status == EnrollmentStatus.Completed)
            throw new ConflictException("This enrollment is already active; no payment is required.");

        var paymentRepository = unitOfWork.Repository<Payment>();

        var hasPendingPayment = await paymentRepository.ExistsAsync(
            p => p.EnrollmentId == enrollment.Id && p.Status == PaymentStatus.Pending, cancellationToken);

        if (hasPendingPayment)
            throw new ConflictException("A pending payment already exists for this course. Wait for it to be reviewed.");

        var receiptPath = $"PaymentReceipts/{studentId}/{Guid.NewGuid():N}{Path.GetExtension(request.ReceiptImageFileName)}";

        var storedReceiptPath = await fileStorageService.UploadAsync(
            request.ReceiptImageStream, receiptPath, request.ReceiptImageContentType, cancellationToken);

        var payment = new Payment
        {
            StudentId = studentId,
            EnrollmentId = enrollment.Id,
            // Server-computed from the course's current price; the client cannot supply an amount.
            Amount = enrollment.Course.DiscountPrice ?? enrollment.Course.Price,
            PaymentMethod = request.PaymentMethod,
            TransactionReference = request.TransactionNumber,
            ReceiptImageUrl = storedReceiptPath,
            Notes = request.Notes,
            Status = PaymentStatus.Pending
        };

        await paymentRepository.AddAsync(payment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await paymentRepository.AsQueryable()
            .Include(p => p.Student)
            .Include(p => p.Enrollment).ThenInclude(e => e.Course)
            .FirstAsync(p => p.Id == payment.Id, cancellationToken);

        return ApiResponse<PaymentDto>.Success(mapper.Map<PaymentDto>(result), "Payment submitted and pending review.");
    }
}
