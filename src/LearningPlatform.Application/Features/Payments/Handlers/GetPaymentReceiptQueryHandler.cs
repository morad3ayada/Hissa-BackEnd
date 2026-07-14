using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Payments.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using MediatR;

namespace LearningPlatform.Application.Features.Payments.Handlers;

public class GetPaymentReceiptQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IFileStorageService fileStorageService)
    : IRequestHandler<GetPaymentReceiptQuery, PaymentReceiptResult>
{
    public async Task<PaymentReceiptResult> Handle(GetPaymentReceiptQuery request, CancellationToken cancellationToken)
    {
        var payment = await unitOfWork.Repository<Payment>().GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), request.PaymentId);

        var isOwner = currentUser.UserId == payment.StudentId;
        var isAdmin = currentUser.IsInRole(nameof(UserRole.Admin));

        if (!isOwner && !isAdmin)
            throw new ForbiddenException("You do not have permission to view this payment.");

        if (string.IsNullOrWhiteSpace(payment.ReceiptImageUrl))
            throw new NotFoundException("This payment has no receipt image.");

        var stream = await fileStorageService.DownloadAsync(payment.ReceiptImageUrl, cancellationToken);
        var fileName = Path.GetFileName(payment.ReceiptImageUrl);
        var contentType = GetContentType(Path.GetExtension(payment.ReceiptImageUrl));

        return new PaymentReceiptResult(stream, contentType, fileName);
    }

    private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".webp" => "image/webp",
        _ => "application/octet-stream"
    };
}
