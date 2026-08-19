using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class CreateUnavailableSlotCommandHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Commands.CreateUnavailableSlotCommand, ApiResponse<UnavailableSlotDto>>
{
    public async Task<ApiResponse<UnavailableSlotDto>> Handle(
        Commands.CreateUnavailableSlotCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (!DateOnly.TryParse(request.Date, out var date))
            throw new BadRequestException("Invalid date format. Use YYYY-MM-DD.");

        if (!TimeOnly.TryParse(request.StartTime, out var startTime) ||
            !TimeOnly.TryParse(request.EndTime, out var endTime))
            throw new BadRequestException("Invalid time format. Use HH:mm.");

        if (startTime >= endTime)
            throw new BadRequestException("Start time must be before end time.");

        var slot = new TeacherUnavailableSlot
        {
            TeacherId = userId,
            Date = date,
            StartTime = startTime,
            EndTime = endTime,
            Reason = request.Reason
        };

        await unitOfWork.Repository<TeacherUnavailableSlot>().AddAsync(slot, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<UnavailableSlotDto>.Success(new UnavailableSlotDto
        {
            Id = slot.Id,
            Date = slot.Date.ToString("yyyy-MM-dd"),
            StartTime = slot.StartTime.ToString("HH:mm"),
            EndTime = slot.EndTime.ToString("HH:mm"),
            Reason = slot.Reason
        }, "Unavailable slot created successfully.");
    }
}
