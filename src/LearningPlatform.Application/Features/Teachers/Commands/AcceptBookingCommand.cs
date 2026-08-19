using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Commands;

public record AcceptBookingCommand(Guid BookingId) : IRequest<ApiResponse>;
