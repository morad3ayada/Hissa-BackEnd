using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Commands;

public record DeleteVideoCommand(Guid LessonId) : IRequest<ApiResponse>;
