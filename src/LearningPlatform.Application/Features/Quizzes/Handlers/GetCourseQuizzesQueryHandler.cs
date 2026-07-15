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

public class GetCourseQuizzesQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<GetCourseQuizzesQuery, ApiResponse<List<QuizSummaryDto>>>
{
    public async Task<ApiResponse<List<QuizSummaryDto>>> Handle(GetCourseQuizzesQuery request, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        var isManager = currentUser.IsInRole(nameof(UserRole.Admin)) || currentUser.UserId == course.InstructorId;

        if (!isManager)
            throw new ForbiddenException("Only the course instructor or admin can view all course quizzes.");

        var quizzes = await unitOfWork.Repository<Quiz>().AsQueryable()
            .Include(q => q.Questions)
            .Where(q => q.CourseId == request.CourseId && q.Scope == QuizScope.Course)
            .ToListAsync(cancellationToken);

        var dtos = quizzes.Select(QuizDtoBuilder.BuildSummary).ToList();

        return ApiResponse<List<QuizSummaryDto>>.Success(dtos);
    }
}
