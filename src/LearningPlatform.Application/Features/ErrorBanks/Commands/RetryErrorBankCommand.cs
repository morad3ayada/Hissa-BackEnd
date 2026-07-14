using LearningPlatform.Application.Features.ErrorBanks.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.ErrorBanks.Commands;

public record RetryAnswerInput(Guid QuestionId, Guid? SelectedAnswerId);

public record RetryErrorBankCommand(List<RetryAnswerInput> Answers) : IRequest<ApiResponse<List<RetryResultDto>>>;
