using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class GetAvailableSlotsQueryHandler(
    IAvailabilityService availabilityService)
    : IRequestHandler<Queries.GetAvailableSlotsQuery, ApiResponse<List<AvailableSlotDto>>>
{
    public async Task<ApiResponse<List<AvailableSlotDto>>> Handle(
        Queries.GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        if (!DateOnly.TryParse(request.Date, out var date))
            throw new BadRequestException("Invalid date format. Use YYYY-MM-DD.");

        if (request.DurationInMinutes <= 0 || request.DurationInMinutes > 480)
            throw new BadRequestException("Duration must be between 1 and 480 minutes.");

        var slots = await availabilityService.GetAvailableSlotsAsync(
            request.TeacherId, date, request.DurationInMinutes, cancellationToken);

        var result = slots.Select(s => new AvailableSlotDto
        {
            Date = s.Date,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList();

        return ApiResponse<List<AvailableSlotDto>>.Success(result);
    }
}
