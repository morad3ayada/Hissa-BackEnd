using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Queries;

public record GetVideoStreamQuery(Guid LessonId) : IRequest<VideoStreamResult>;

public record VideoStreamResult(Stream Stream, string ContentType, string FileName);
