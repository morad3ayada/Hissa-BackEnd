using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Mappings;
using LearningPlatform.Application.Features.Quizzes.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class GetAllPublishedCourseQuizzesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllPublishedCourseQuizzesQuery, ApiResponse<List<QuizSummaryDto>>>
{
    public async Task<ApiResponse<List<QuizSummaryDto>>> Handle(
        GetAllPublishedCourseQuizzesQuery request, CancellationToken cancellationToken)
    {
        // Make sure course exists
        var courseExists = await unitOfWork.Repository<Course>().ExistsAsync(
            c => c.Id == request.CourseId, cancellationToken);
        if (!courseExists)
            throw new NotFoundException(nameof(Course), request.CourseId);

        // Get all published quizzes whose CourseId matches, OR whose Lesson belongs to this course
        // Get all lesson IDs belonging to this course
        var lessonIds = await unitOfWork.Repository<Lesson>().AsQueryable()
            .Include(l => l.CourseSection)
            .Where(l => l.CourseSection.CourseId == request.CourseId)
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);

        var quizzes = await unitOfWork.Repository<Quiz>().AsQueryable()
            .Include(q => q.Questions)
            .Where(q => q.IsPublished &&
                        (q.CourseId == request.CourseId ||
                         (q.LessonId != null && lessonIds.Contains(q.LessonId.Value))))
            .ToListAsync(cancellationToken);

        var dtos = quizzes.Select(QuizDtoBuilder.BuildSummary).ToList();
        return ApiResponse<List<QuizSummaryDto>>.Success(dtos);
    }
}
