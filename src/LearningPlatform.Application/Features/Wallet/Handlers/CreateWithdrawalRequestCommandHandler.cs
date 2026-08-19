using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Wallet.Commands;
using LearningPlatform.Application.Features.Wallet.Common;
using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Application.Features.Wallet.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Wallet.Handlers;

public class CreateWithdrawalRequestCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IWalletService walletService)
    : IRequestHandler<CreateWithdrawalRequestCommand, ApiResponse<WalletRequestDto>>
{
    public async Task<ApiResponse<WalletRequestDto>> Handle(CreateWithdrawalRequestCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            throw new BadRequestException("Withdrawal amount must be greater than zero.");

        var wallet = await walletService.GetOrCreateAsync(currentUser.UserId!.Value, cancellationToken);

        if (wallet.Balance < request.Amount)
            throw new BadRequestException("Insufficient wallet balance for this withdrawal.");

        var withdrawalRequest = new WalletRequest
        {
            WalletId = wallet.Id,
            Type = WalletRequestType.Withdrawal,
            Amount = request.Amount
        };

        await unitOfWork.Repository<WalletRequest>().AddAsync(withdrawalRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<WalletRequestDto>.Success(
            WalletMapper.ToDto(withdrawalRequest),
            "Withdrawal request submitted and is pending admin review.");
    }
}
