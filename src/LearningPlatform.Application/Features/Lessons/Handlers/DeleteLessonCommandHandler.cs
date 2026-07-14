using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Lessons.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Handlers;

public class DeleteLessonCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IFileStorageService fileStorageService,
    ICourseDurationRecalculator durationRecalculator)
    : IRequestHandler<DeleteLessonCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
    {
        var lessonRepository = unitOfWork.Repository<Lesson>();

        var lesson = await lessonRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Lesson), request.Id);

        var section = await unitOfWork.Repository<CourseSection>().GetByIdAsync(lesson.CourseSectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), lesson.CourseSectionId);

        var course = await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), section.CourseId);

        currentUser.EnsureCanManageCourse(course);

        var videoPath = lesson.ContentUrl;

        lessonRepository.Remove(lesson);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(videoPath))
            await fileStorageService.DeleteAsync(videoPath, cancellationToken);

        await durationRecalculator.RecalculateAsync(course.Id, cancellationToken);

        return ApiResponse.Success("Lesson deleted successfully.");
    }
}
