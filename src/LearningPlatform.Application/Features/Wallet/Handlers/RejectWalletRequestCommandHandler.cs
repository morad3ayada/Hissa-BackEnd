using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Wallet.Commands;
using LearningPlatform.Application.Features.Wallet.Common;
using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Wallet.Handlers;

public class RejectWalletRequestCommandHandler(
    IUnitOfWork unitOfWork,
    INotificationService notificationService)
    : IRequestHandler<RejectWalletRequestCommand, ApiResponse<WalletRequestDto>>
{
    public async Task<ApiResponse<WalletRequestDto>> Handle(RejectWalletRequestCommand request, CancellationToken cancellationToken)
    {
        var walletRequest = await unitOfWork.Repository<WalletRequest>()
            .GetTrackedAsync(r => r.Id == request.RequestId, cancellationToken)
            ?? throw new NotFoundException(nameof(WalletRequest), request.RequestId);

        if (walletRequest.Status != WalletRequestStatus.Pending)
            throw new BadRequestException("Only pending wallet requests can be rejected.");

        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new BadRequestException("A rejection reason is required.");

        var wallet = await unitOfWork.Repository<LearningPlatform.Domain.Entities.Wallet>()
            .GetTrackedAsync(w => w.Id == walletRequest.WalletId, cancellationToken)
            ?? throw new NotFoundException(nameof(Wallet), walletRequest.WalletId);

        walletRequest.Status = WalletRequestStatus.Rejected;
        walletRequest.RejectionReason = request.Reason.Trim();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await notificationService.CreateAsync(
            wallet.StudentId,
            NotificationType.Warning,
            "Wallet request rejected",
            $"Your {walletRequest.Type.ToString().ToLowerInvariant()} request of {walletRequest.Amount:0.##} EGP was rejected: {walletRequest.RejectionReason}",
            "/wallet",
            cancellationToken);

        return ApiResponse<WalletRequestDto>.Success(
            WalletMapper.ToDto(walletRequest),
            "Wallet request rejected.");
    }
}
