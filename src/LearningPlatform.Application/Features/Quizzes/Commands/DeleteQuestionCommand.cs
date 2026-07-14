using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Commands;

public record DeleteQuestionCommand(Guid QuestionId) : IRequest<ApiResponse>;
