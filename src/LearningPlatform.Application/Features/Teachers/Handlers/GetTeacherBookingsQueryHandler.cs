using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class GetTeacherBookingsQueryHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetTeacherBookingsQuery, ApiResponse<List<BookingDto>>>
{
    public async Task<ApiResponse<List<BookingDto>>> Handle(
        Queries.GetTeacherBookingsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var query = unitOfWork.Repository<Booking>()
            .AsQueryable()
            .Include(b => b.Student)
            .Where(b => b.TeacherId == userId);

        if (request.Status.HasValue)
            query = query.Where(b => b.Status == request.Status.Value);

        if (request.From.HasValue)
            query = query.Where(b => b.Date >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(b => b.Date <= request.To.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var bookings = await query
            .OrderByDescending(b => b.Date)
            .ThenBy(b => b.StartTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var result = bookings.Select(b => new BookingDto
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

        return ApiResponse<List<BookingDto>>.Success(result);
    }
}
