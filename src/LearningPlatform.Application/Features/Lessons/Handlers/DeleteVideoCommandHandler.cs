using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Lessons.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Handlers;

public class DeleteVideoCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IFileStorageService fileStorageService,
    ICourseDurationRecalculator durationRecalculator)
    : IRequestHandler<DeleteVideoCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
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
            throw new NotFoundException("This lesson has no video to delete.");

        var videoPath = lesson.ContentUrl;

        lesson.ContentUrl = null;
        lesson.DurationInSeconds = null;
        lessonRepository.Update(lesson);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await fileStorageService.DeleteAsync(videoPath, cancellationToken);
        await durationRecalculator.RecalculateAsync(course.Id, cancellationToken);

        return ApiResponse.Success("Video deleted successfully.");
    }
}
