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

public class CreateLessonCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ICourseDurationRecalculator durationRecalculator,
    IMapper mapper)
    : IRequestHandler<CreateLessonCommand, ApiResponse<LessonDto>>
{
    public async Task<ApiResponse<LessonDto>> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
    {
        var section = await unitOfWork.Repository<CourseSection>().GetByIdAsync(request.CourseSectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), request.CourseSectionId);

        var course = await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), section.CourseId);

        currentUser.EnsureCanManageCourse(course);

        var lessonRepository = unitOfWork.Repository<Lesson>();
        var existingLessons = await lessonRepository.FindAsync(l => l.CourseSectionId == request.CourseSectionId, cancellationToken);
        var nextOrder = existingLessons.Count == 0 ? 1 : existingLessons.Max(l => l.Order) + 1;

        var lesson = new Lesson
        {
            CourseSectionId = request.CourseSectionId,
            Title = request.Title,
            Type = request.Type,
            Content = request.Content,
            DurationInSeconds = request.DurationInSeconds,
            IsFreePreview = request.IsFreePreview,
            Order = nextOrder
        };

        await lessonRepository.AddAsync(lesson, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (request.DurationInSeconds is > 0)
            await durationRecalculator.RecalculateAsync(course.Id, cancellationToken);

        return ApiResponse<LessonDto>.Success(mapper.Map<LessonDto>(lesson), "Lesson created successfully.");
    }
}
