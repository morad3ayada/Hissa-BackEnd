using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Mappings;
using LearningPlatform.Application.Features.Quizzes.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class GetLessonQuizzesQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ILessonAccessService lessonAccessService)
    : IRequestHandler<GetLessonQuizzesQuery, ApiResponse<List<QuizSummaryDto>>>
{
    public async Task<ApiResponse<List<QuizSummaryDto>>> Handle(GetLessonQuizzesQuery request, CancellationToken cancellationToken)
    {
        var lesson = await unitOfWork.Repository<Lesson>().GetByIdAsync(request.LessonId, cancellationToken)
            ?? throw new NotFoundException(nameof(Lesson), request.LessonId);

        var section = await unitOfWork.Repository<CourseSection>().GetByIdAsync(lesson.CourseSectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), lesson.CourseSectionId);

        var course = await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), section.CourseId);

        var isManager = currentUser.IsInRole(nameof(UserRole.Admin)) || currentUser.UserId == course.InstructorId;

        if (!isManager)
            await lessonAccessService.EnsureCanViewLessonAsync(course, lesson, cancellationToken);

        var quizzesQuery = unitOfWork.Repository<Quiz>().AsQueryable()
            .Include(q => q.Questions)
            .Where(q => q.LessonId == request.LessonId);

        if (!isManager)
            quizzesQuery = quizzesQuery.Where(q => q.IsPublished);

        var quizzes = await quizzesQuery.ToListAsync(cancellationToken);

        var dtos = quizzes.Select(QuizDtoBuilder.BuildSummary).ToList();

        return ApiResponse<List<QuizSummaryDto>>.Success(dtos);
    }
}
