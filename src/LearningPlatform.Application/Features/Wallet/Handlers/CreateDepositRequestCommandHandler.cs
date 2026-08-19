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

public class CreateDepositRequestCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IWalletService walletService)
    : IRequestHandler<CreateDepositRequestCommand, ApiResponse<WalletRequestDto>>
{
    public async Task<ApiResponse<WalletRequestDto>> Handle(CreateDepositRequestCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            throw new BadRequestException("Deposit amount must be greater than zero.");

        var wallet = await walletService.GetOrCreateAsync(currentUser.UserId!.Value, cancellationToken);

        var depositRequest = new WalletRequest
        {
            WalletId = wallet.Id,
            Type = WalletRequestType.Deposit,
            Amount = request.Amount,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };

        await unitOfWork.Repository<WalletRequest>().AddAsync(depositRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<WalletRequestDto>.Success(
            WalletMapper.ToDto(depositRequest),
            "Deposit request submitted and is pending admin review.");
    }
}
