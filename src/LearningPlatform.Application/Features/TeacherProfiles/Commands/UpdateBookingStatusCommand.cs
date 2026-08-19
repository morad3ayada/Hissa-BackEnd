using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.TeacherProfiles.Commands;

public record UpdateBookingStatusCommand : IRequest<ApiResponse>
{
    public bool AcceptingBookings { get; init; }
}
