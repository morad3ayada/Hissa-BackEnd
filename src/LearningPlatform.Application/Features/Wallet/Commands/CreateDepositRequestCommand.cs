using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Wallet.Commands;

public record CreateDepositRequestCommand(decimal Amount, string? Notes) : IRequest<ApiResponse<WalletRequestDto>>;
