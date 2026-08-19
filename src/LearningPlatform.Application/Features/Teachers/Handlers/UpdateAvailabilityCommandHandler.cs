using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class UpdateAvailabilityCommandHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Commands.UpdateAvailabilityCommand, ApiResponse>
{
    private static readonly Dictionary<string, int> DayMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Sunday"] = 0, ["Monday"] = 1, ["Tuesday"] = 2, ["Wednesday"] = 3,
        ["Thursday"] = 4, ["Friday"] = 5, ["Saturday"] = 6
    };

    public async Task<ApiResponse> Handle(
        Commands.UpdateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var existing = await unitOfWork.Repository<TeacherAvailability>()
            .AsQueryable()
            .Where(a => a.TeacherId == userId)
            .ToListAsync(cancellationToken);

        foreach (var slot in existing)
            unitOfWork.Repository<TeacherAvailability>().Delete(slot);

        foreach (var dto in request.Availability)
        {
            if (!DayMap.TryGetValue(dto.Day, out var dayOfWeek))
                throw new Shared.Exceptions.BadRequestException($"Invalid day: {dto.Day}");

            if (!TimeOnly.TryParse(dto.StartTime, out var startTime) ||
                !TimeOnly.TryParse(dto.EndTime, out var endTime))
                throw new Shared.Exceptions.BadRequestException("Invalid time format. Use HH:mm.");

            if (startTime >= endTime)
                throw new Shared.Exceptions.BadRequestException($"Start time must be before end time for {dto.Day}.");

            await unitOfWork.Repository<TeacherAvailability>().AddAsync(new TeacherAvailability
            {
                TeacherId = userId,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime
            }, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Availability updated successfully.");
    }
}
