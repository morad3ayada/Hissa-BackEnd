using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Queries;

public record GetTeacherCalendarQuery : IRequest<ApiResponse<List<CalendarEventDto>>>
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public Domain.Enums.BookingStatus? Status { get; init; }
}
