using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Lessons.DTOs;
using LearningPlatform.Application.Features.Lessons.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Handlers;

public class GetLessonQueryHandler(
    IUnitOfWork unitOfWork,
    ILessonAccessService lessonAccessService,
    IMapper mapper)
    : IRequestHandler<GetLessonQuery, ApiResponse<LessonDto>>
{
    public async Task<ApiResponse<LessonDto>> Handle(GetLessonQuery request, CancellationToken cancellationToken)
    {
        var lesson = await unitOfWork.Repository<Lesson>().GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Lesson), request.Id);

        var section = await unitOfWork.Repository<CourseSection>().GetByIdAsync(lesson.CourseSectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), lesson.CourseSectionId);

        var course = await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), section.CourseId);

        await lessonAccessService.EnsureCanViewLessonAsync(course, lesson, cancellationToken);

        return ApiResponse<LessonDto>.Success(mapper.Map<LessonDto>(lesson));
    }
}
