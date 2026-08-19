using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Wallet.Queries;

public record GetWalletSummaryQuery : IRequest<ApiResponse<WalletSummaryDto>>;
