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

public class ApproveWalletRequestCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider,
    INotificationService notificationService)
    : IRequestHandler<ApproveWalletRequestCommand, ApiResponse<WalletRequestDto>>
{
    public async Task<ApiResponse<WalletRequestDto>> Handle(ApproveWalletRequestCommand request, CancellationToken cancellationToken)
    {
        var walletRequest = await unitOfWork.Repository<WalletRequest>()
            .GetTrackedAsync(r => r.Id == request.RequestId, cancellationToken)
            ?? throw new NotFoundException(nameof(WalletRequest), request.RequestId);

        if (walletRequest.Status != WalletRequestStatus.Pending)
            throw new BadRequestException("Only pending wallet requests can be approved.");

        var wallet = await unitOfWork.Repository<LearningPlatform.Domain.Entities.Wallet>()
            .GetTrackedAsync(w => w.Id == walletRequest.WalletId, cancellationToken)
            ?? throw new NotFoundException(nameof(Wallet), walletRequest.WalletId);

        var now = dateTimeProvider.UtcNow;

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            if (walletRequest.Type == WalletRequestType.Deposit)
            {
                wallet.Balance += walletRequest.Amount;
            }
            else
            {
                if (wallet.Balance < walletRequest.Amount)
                    throw new BadRequestException("Insufficient wallet balance for this withdrawal.");

                wallet.Balance -= walletRequest.Amount;
            }

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Type = walletRequest.Type == WalletRequestType.Deposit
                    ? WalletTransactionType.Deposit
                    : WalletTransactionType.Withdrawal,
                Amount = walletRequest.Amount,
                BalanceAfter = wallet.Balance,
                ReferenceId = walletRequest.Id,
                Notes = walletRequest.Notes
            };

            await unitOfWork.Repository<WalletTransaction>().AddAsync(transaction, cancellationToken);

            walletRequest.Status = WalletRequestStatus.Approved;
            walletRequest.ApprovedAt = now;
            walletRequest.ApprovedById = currentUser.UserId;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await notificationService.CreateAsync(
                wallet.StudentId,
                NotificationType.Success,
                "Wallet request approved",
                walletRequest.Type == WalletRequestType.Deposit
                    ? $"Your deposit of {walletRequest.Amount:0.##} EGP was added to your wallet."
                    : $"Your withdrawal of {walletRequest.Amount:0.##} EGP was completed.",
                "/wallet",
                cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        return ApiResponse<WalletRequestDto>.Success(
            WalletMapper.ToDto(walletRequest),
            walletRequest.Type == WalletRequestType.Deposit
                ? "Deposit request approved."
                : "Withdrawal request approved.");
    }
}
