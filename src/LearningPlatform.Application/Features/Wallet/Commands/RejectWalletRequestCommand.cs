using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Wallet.Commands;

public record RejectWalletRequestCommand(Guid RequestId, string Reason) : IRequest<ApiResponse<WalletRequestDto>>;
