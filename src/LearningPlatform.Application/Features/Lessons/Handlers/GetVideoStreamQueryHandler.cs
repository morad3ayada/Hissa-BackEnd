using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Lessons.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Handlers;

public class GetVideoStreamQueryHandler(
    IUnitOfWork unitOfWork,
    ILessonAccessService lessonAccessService,
    IFileStorageService fileStorageService)
    : IRequestHandler<GetVideoStreamQuery, VideoStreamResult>
{
    public async Task<VideoStreamResult> Handle(GetVideoStreamQuery request, CancellationToken cancellationToken)
    {
        var lesson = await unitOfWork.Repository<Lesson>().GetByIdAsync(request.LessonId, cancellationToken)
            ?? throw new NotFoundException(nameof(Lesson), request.LessonId);

        var section = await unitOfWork.Repository<CourseSection>().GetByIdAsync(lesson.CourseSectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), lesson.CourseSectionId);

        var course = await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), section.CourseId);

        await lessonAccessService.EnsureCanViewLessonAsync(course, lesson, cancellationToken);

        if (string.IsNullOrWhiteSpace(lesson.ContentUrl))
            throw new NotFoundException("This lesson has no video.");

        var stream = await fileStorageService.DownloadAsync(lesson.ContentUrl, cancellationToken);
        var fileName = Path.GetFileName(lesson.ContentUrl);
        var contentType = GetContentType(Path.GetExtension(lesson.ContentUrl));

        return new VideoStreamResult(stream, contentType, fileName);
    }

    private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".mp4" => "video/mp4",
        ".mov" => "video/quicktime",
        ".avi" => "video/x-msvideo",
        ".mkv" => "video/x-matroska",
        ".webm" => "video/webm",
        _ => "application/octet-stream"
    };
}
