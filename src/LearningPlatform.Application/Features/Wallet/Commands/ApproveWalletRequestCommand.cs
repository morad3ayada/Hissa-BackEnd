using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Wallet.Commands;

public record ApproveWalletRequestCommand(Guid RequestId) : IRequest<ApiResponse<WalletRequestDto>>;
