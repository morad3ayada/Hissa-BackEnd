using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Commands;

public record DeleteLessonCommand(Guid Id) : IRequest<ApiResponse>;
