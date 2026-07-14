using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Commands;

public record PublishQuizCommand(Guid QuizId) : IRequest<ApiResponse<QuizDto>>;
