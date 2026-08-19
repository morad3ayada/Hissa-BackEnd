using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class GetBookingDetailQueryHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetBookingDetailQuery, ApiResponse<BookingDetailDto>>
{
    public async Task<ApiResponse<BookingDetailDto>> Handle(
        Queries.GetBookingDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var booking = await unitOfWork.Repository<Booking>()
            .AsQueryable()
            .Include(b => b.Student)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");

        if (booking.TeacherId != userId)
            throw new ForbiddenException("You can only view your own bookings.");

        var dto = new BookingDetailDto
        {
            BookingId = booking.Id,
            StudentName = $"{booking.Student.FirstName} {booking.Student.LastName}",
            StudentImage = booking.Student.ProfilePictureUrl,
            Subject = booking.Subject,
            Date = booking.Date,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            DurationInMinutes = booking.DurationInMinutes,
            Price = booking.Price,
            Status = booking.Status,
            Notes = booking.Notes,
            CancellationReason = booking.CancellationReason,
            CreatedAt = booking.CreatedAt
        };

        return ApiResponse<BookingDetailDto>.Success(dto);
    }
}
