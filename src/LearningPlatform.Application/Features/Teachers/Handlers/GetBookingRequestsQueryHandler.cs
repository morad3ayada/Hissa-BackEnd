using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class GetBookingRequestsQueryHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetBookingRequestsQuery, ApiResponse<List<BookingDto>>>
{
    public async Task<ApiResponse<List<BookingDto>>> Handle(
        Queries.GetBookingRequestsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var bookings = await unitOfWork.Repository<Booking>()
            .AsQueryable()
            .Include(b => b.Student)
            .Where(b => b.TeacherId == userId && b.Status == BookingStatus.Pending)
            .OrderBy(b => b.Date)
            .ThenBy(b => b.StartTime)
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
