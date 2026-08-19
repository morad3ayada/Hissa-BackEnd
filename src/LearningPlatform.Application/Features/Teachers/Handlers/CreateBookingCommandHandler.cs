using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class CreateBookingCommandHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork,
    IAvailabilityService availabilityService)
    : IRequestHandler<Commands.CreateBookingCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(
        Commands.CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (!DateOnly.TryParse(request.Date, out var date))
            throw new BadRequestException("Invalid date format. Use YYYY-MM-DD.");

        if (!TimeOnly.TryParse(request.StartTime, out var startTime))
            throw new BadRequestException("Invalid time format. Use HH:mm.");

        var endTime = startTime.AddMinutes(request.DurationInMinutes);

        var profile = await unitOfWork.Repository<TeacherProfile>()
            .AsQueryable()
            .FirstOrDefaultAsync(t => t.UserId == request.TeacherId, cancellationToken)
            ?? throw new NotFoundException("Teacher not found.");

        if (!profile.CanReceiveBookings)
            throw new BadRequestException("This teacher is not currently accepting bookings.");

        var isAvailable = await availabilityService.IsSlotAvailableAsync(
            request.TeacherId, date, startTime, endTime, cancellationToken);

        if (!isAvailable)
            throw new BadRequestException("The requested time slot is not available.");

        var price = profile.LessonPrice ?? 0;

        var booking = new Booking
        {
            TeacherId = request.TeacherId,
            StudentId = userId,
            Date = date,
            StartTime = startTime,
            EndTime = endTime,
            Subject = request.Subject,
            DurationInMinutes = request.DurationInMinutes,
            Price = price,
            Status = BookingStatus.Pending,
            Notes = request.Notes
        };

        await unitOfWork.Repository<Booking>().AddAsync(booking, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Booking created successfully. Awaiting teacher confirmation.");
    }
}
