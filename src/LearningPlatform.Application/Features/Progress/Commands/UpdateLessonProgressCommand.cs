using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Progress.Commands;

public record UpdateLessonProgressCommand : IRequest<ApiResponse<LessonProgressDto>>
{
    public Guid LessonId { get; init; }
    public int CurrentSecond { get; init; }
    public decimal WatchPercentage { get; init; }
}
