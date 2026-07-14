using LearningPlatform.Application.Features.Lessons.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Commands;

public record UpdateLessonCommand : IRequest<ApiResponse<LessonDto>>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public LessonType Type { get; init; }
    public string? Content { get; init; }
    public int? DurationInSeconds { get; init; }
    public int Order { get; init; }
    public bool IsFreePreview { get; init; }
}
