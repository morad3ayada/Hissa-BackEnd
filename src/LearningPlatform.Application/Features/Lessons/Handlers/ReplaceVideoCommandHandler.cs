using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Lessons.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Handlers;

public class ReplaceVideoCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IFileStorageService fileStorageService,
    ICourseDurationRecalculator durationRecalculator)
    : IRequestHandler<ReplaceVideoCommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(ReplaceVideoCommand request, CancellationToken cancellationToken)
    {
        var lessonRepository = unitOfWork.Repository<Lesson>();

        var lesson = await lessonRepository.GetByIdAsync(request.LessonId, cancellationToken)
            ?? throw new NotFoundException(nameof(Lesson), request.LessonId);

        var section = await unitOfWork.Repository<CourseSection>().GetByIdAsync(lesson.CourseSectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), lesson.CourseSectionId);

        var course = await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), section.CourseId);

        currentUser.EnsureCanManageCourse(course);

        if (string.IsNullOrWhiteSpace(lesson.ContentUrl))
            throw new NotFoundException("This lesson has no existing video. Use the upload-video endpoint instead.");

        var previousPath = lesson.ContentUrl;
        var relativePath = UploadVideoCommandHandler.BuildVideoPath(course.Title, section.Title, lesson.Title, request.FileName);

        var storedPath = await fileStorageService.UploadAsync(
            request.FileStream, relativePath, request.ContentType, cancellationToken);

        lesson.ContentUrl = storedPath;
        if (request.DurationInSeconds.HasValue)
            lesson.DurationInSeconds = request.DurationInSeconds;

        lessonRepository.Update(lesson);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (previousPath != storedPath)
            await fileStorageService.DeleteAsync(previousPath, cancellationToken);

        await durationRecalculator.RecalculateAsync(course.Id, cancellationToken);

        return ApiResponse<string>.Success(storedPath, "Video replaced successfully.");
    }
}
