using AutoMapper;
using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Lessons.Commands;
using LearningPlatform.Application.Features.Lessons.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Handlers;

public class UpdateLessonCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ICourseDurationRecalculator durationRecalculator,
    IMapper mapper)
    : IRequestHandler<UpdateLessonCommand, ApiResponse<LessonDto>>
{
    public async Task<ApiResponse<LessonDto>> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
    {
        var lessonRepository = unitOfWork.Repository<Lesson>();

        var lesson = await lessonRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Lesson), request.Id);

        var section = await unitOfWork.Repository<CourseSection>().GetByIdAsync(lesson.CourseSectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), lesson.CourseSectionId);

        var course = await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), section.CourseId);

        currentUser.EnsureCanManageCourse(course);

        var durationChanged = lesson.DurationInSeconds != request.DurationInSeconds;

        lesson.Title = request.Title;
        lesson.Type = request.Type;
        lesson.Content = request.Content;
        lesson.DurationInSeconds = request.DurationInSeconds;
        lesson.Order = request.Order;
        lesson.IsFreePreview = request.IsFreePreview;

        lessonRepository.Update(lesson);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (durationChanged)
            await durationRecalculator.RecalculateAsync(course.Id, cancellationToken);

        return ApiResponse<LessonDto>.Success(mapper.Map<LessonDto>(lesson), "Lesson updated successfully.");
    }
}
