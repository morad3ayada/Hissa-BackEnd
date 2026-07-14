using LearningPlatform.Application.Features.ErrorBanks.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.ErrorBanks.Queries;

public record GetMyErrorsQuery(bool IncludeResolved = false) : IRequest<ApiResponse<List<ErrorBankEntryDto>>>;
