using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Commands;

public record RejectBookingCommand : IRequest<ApiResponse>
{
    public Guid BookingId { get; init; }
    public string? Reason { get; init; }
}
