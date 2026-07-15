using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class GetEnrolledCoursesQuizzesQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<GetEnrolledCoursesQuizzesQuery, ApiResponse<List<EnrolledCourseQuizSummaryDto>>>
{
    public async Task<ApiResponse<List<EnrolledCourseQuizSummaryDto>>> Handle(
        GetEnrolledCoursesQuizzesQuery request,
        CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId
            ?? throw new UnauthorizedException();

        // جلب IDs الكورسات التي الطالب مشترك فيها وبحالة Active
        var enrolledCourseIds = await unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Active)
            .Select(e => e.CourseId)
            .ToListAsync(cancellationToken);

        if (enrolledCourseIds.Count == 0)
            return ApiResponse<List<EnrolledCourseQuizSummaryDto>>.Success([]);

        // جلب كل الـ Quizzes المنشورة التابعة لـ Lessons داخل هذه الكورسات
        var quizzes = await unitOfWork.Repository<Quiz>()
            .AsQueryable()
            .Where(q =>
                q.IsPublished &&
                q.Scope == QuizScope.Lesson &&
                q.LessonId != null &&
                q.Lesson != null &&
                q.Lesson.CourseSection != null &&
                enrolledCourseIds.Contains(q.Lesson.CourseSection.CourseId))
            .Include(q => q.Questions)
            .Include(q => q.Lesson)
                .ThenInclude(l => l!.CourseSection)
                    .ThenInclude(s => s.Course)
            .OrderBy(q => q.Lesson!.CourseSection.Course.Title)
            .ThenBy(q => q.Lesson!.Order)
            .ThenBy(q => q.Title)
            .ToListAsync(cancellationToken);

        var dtos = quizzes.Select(q => new EnrolledCourseQuizSummaryDto
        {
            CourseId    = q.Lesson!.CourseSection.CourseId,
            CourseTitle = q.Lesson.CourseSection.Course.Title,
            LessonId    = q.LessonId!.Value,
            LessonTitle = q.Lesson.Title,
            QuizId      = q.Id,
            QuizTitle   = q.Title,
            TimeLimitInMinutes = q.TimeLimitInMinutes,
            PassingScore       = q.PassingScore,
            MaxAttempts        = q.MaxAttempts,
            QuestionsCount     = q.Questions.Count
        }).ToList();

        return ApiResponse<List<EnrolledCourseQuizSummaryDto>>.Success(dtos);
    }
}
