using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Lessons.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Helpers;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Handlers;

public class UploadVideoCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IFileStorageService fileStorageService,
    ICourseDurationRecalculator durationRecalculator)
    : IRequestHandler<UploadVideoCommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(UploadVideoCommand request, CancellationToken cancellationToken)
    {
        var lessonRepository = unitOfWork.Repository<Lesson>();

        var lesson = await lessonRepository.GetByIdAsync(request.LessonId, cancellationToken)
            ?? throw new NotFoundException(nameof(Lesson), request.LessonId);

        var section = await unitOfWork.Repository<CourseSection>().GetByIdAsync(lesson.CourseSectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), lesson.CourseSectionId);

        var course = await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), section.CourseId);

        currentUser.EnsureCanManageCourse(course);

        if (!string.IsNullOrWhiteSpace(lesson.ContentUrl))
            throw new ConflictException("This lesson already has a video. Use the replace-video endpoint instead.");

        var relativePath = BuildVideoPath(course.Title, section.Title, lesson.Title, request.FileName);

        var storedPath = await fileStorageService.UploadAsync(
            request.FileStream, relativePath, request.ContentType, cancellationToken);

        lesson.ContentUrl = storedPath;
        if (request.DurationInSeconds.HasValue)
            lesson.DurationInSeconds = request.DurationInSeconds;

        lessonRepository.Update(lesson);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await durationRecalculator.RecalculateAsync(course.Id, cancellationToken);

        return ApiResponse<string>.Success(storedPath, "Video uploaded successfully.");
    }

    internal static string BuildVideoPath(string courseTitle, string sectionTitle, string lessonTitle, string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var courseFolder = SlugGenerator.ToSafeFolderName(courseTitle);
        var sectionFolder = SlugGenerator.ToSafeFolderName(sectionTitle);
        var fileName = SlugGenerator.ToSafeFolderName(lessonTitle);

        return $"PrivateVideos/{courseFolder}/{sectionFolder}/{fileName}{extension}";
    }
}
