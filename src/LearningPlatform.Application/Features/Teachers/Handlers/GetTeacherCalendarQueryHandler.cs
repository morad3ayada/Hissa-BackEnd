using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class GetTeacherCalendarQueryHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetTeacherCalendarQuery, ApiResponse<List<CalendarEventDto>>>
{
    public async Task<ApiResponse<List<CalendarEventDto>>> Handle(
        Queries.GetTeacherCalendarQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var query = unitOfWork.Repository<Booking>()
            .AsQueryable()
            .Include(b => b.Student)
            .Where(b => b.TeacherId == userId);

        if (request.From.HasValue)
            query = query.Where(b => b.Date >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(b => b.Date <= request.To.Value);

        if (request.Status.HasValue)
            query = query.Where(b => b.Status == request.Status.Value);

        var bookings = await query
            .OrderBy(b => b.Date)
            .ThenBy(b => b.StartTime)
            .ToListAsync(cancellationToken);

        var events = bookings.Select(b => new CalendarEventDto
        {
            BookingId = b.Id,
            StudentName = $"{b.Student.FirstName} {b.Student.LastName}",
            StudentImage = b.Student.ProfilePictureUrl,
            Subject = b.Subject,
            Date = b.Date,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            DurationInMinutes = b.DurationInMinutes,
            Price = b.Price,
            Status = b.Status
        }).ToList();

        return ApiResponse<List<CalendarEventDto>>.Success(events);
    }
}
