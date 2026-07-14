using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Commands;

public record ReplaceVideoCommand : IRequest<ApiResponse<string>>
{
    public Guid LessonId { get; init; }
    public Stream FileStream { get; init; } = Stream.Null;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public int? DurationInSeconds { get; init; }
}
