using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Application.Features.Gamification.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Gamification.Handlers;

public class GetLeaderboardQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetLeaderboardQuery, PaginatedResponse<LeaderboardEntryDto>>
{
    public async Task<PaginatedResponse<LeaderboardEntryDto>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        IQueryable<GamificationProfile> profilesQuery = unitOfWork.Repository<GamificationProfile>().AsQueryable()
            .Include(p => p.Student);

        if (request.CourseId.HasValue)
        {
            var enrolledStudentIds = await unitOfWork.Repository<Enrollment>().AsQueryable()
                .Where(e => e.CourseId == request.CourseId.Value &&
                    (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed))
                .Select(e => e.StudentId)
                .Distinct()
                .ToListAsync(cancellationToken);

            profilesQuery = profilesQuery.Where(p => enrolledStudentIds.Contains(p.StudentId));
        }

        var totalCount = await profilesQuery.CountAsync(cancellationToken);

        var pagedProfiles = await profilesQuery
            .OrderByDescending(p => p.TotalPoints)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var studentIds = pagedProfiles.Select(p => p.StudentId).ToList();

        var completedCoursesByStudent = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Where(e => studentIds.Contains(e.StudentId) && e.Status == EnrollmentStatus.Completed)
            .GroupBy(e => e.StudentId)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.StudentId, g => g.Count, cancellationToken);

        var passedQuizzesByStudent = await unitOfWork.Repository<QuizResult>().AsQueryable()
            .Where(r => studentIds.Contains(r.StudentId) && r.IsPassed)
            .GroupBy(r => r.StudentId)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.StudentId, g => g.Count, cancellationToken);

        var entries = pagedProfiles.Select((p, index) => new LeaderboardEntryDto
        {
            Rank = (request.PageNumber - 1) * request.PageSize + index + 1,
            StudentId = p.StudentId,
            StudentName = $"{p.Student.FirstName} {p.Student.LastName}",
            ProfilePictureUrl = p.Student.ProfilePictureUrl,
            TotalPoints = p.TotalPoints,
            CurrentLevel = p.CurrentLevel,
            CompletedCoursesCount = completedCoursesByStudent.GetValueOrDefault(p.StudentId),
            PassedQuizzesCount = passedQuizzesByStudent.GetValueOrDefault(p.StudentId)
        }).ToList();

        var paginatedList = new PaginatedList<LeaderboardEntryDto>(entries, totalCount, request.PageNumber, request.PageSize);

        return PaginatedResponse<LeaderboardEntryDto>.Create(paginatedList);
    }
}
