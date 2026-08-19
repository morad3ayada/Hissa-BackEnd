using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class GetTeacherAvailabilityQueryHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetTeacherAvailabilityQuery, ApiResponse<List<TeacherAvailabilityDto>>>
{
    private static readonly string[] DayNames =
        ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

    public async Task<ApiResponse<List<TeacherAvailabilityDto>>> Handle(
        Queries.GetTeacherAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var slots = await unitOfWork.Repository<TeacherAvailability>()
            .AsQueryable()
            .Where(a => a.TeacherId == userId)
            .OrderBy(a => a.DayOfWeek)
            .ThenBy(a => a.StartTime)
            .ToListAsync(cancellationToken);

        var result = slots.Select(s => new TeacherAvailabilityDto
        {
            Day = DayNames[s.DayOfWeek],
            StartTime = s.StartTime.ToString("HH:mm"),
            EndTime = s.EndTime.ToString("HH:mm")
        }).ToList();

        return ApiResponse<List<TeacherAvailabilityDto>>.Success(result);
    }
}
