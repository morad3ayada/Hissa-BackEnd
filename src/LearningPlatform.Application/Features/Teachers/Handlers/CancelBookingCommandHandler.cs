using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class CancelBookingCommandHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Commands.CancelBookingCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(
        Commands.CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var booking = await unitOfWork.Repository<Booking>()
            .GetByIdAsync(request.BookingId, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");

        if (booking.TeacherId != userId)
            throw new ForbiddenException("You can only manage your own bookings.");

        if (booking.Status is BookingStatus.Completed or BookingStatus.Cancelled)
            throw new BadRequestException("This booking cannot be cancelled.");

        booking.Status = BookingStatus.Cancelled;
        booking.CancellationReason = request.Reason;
        booking.CancelledAt = DateTime.UtcNow;

        unitOfWork.Repository<Booking>().Update(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Booking cancelled successfully.");
    }
}
