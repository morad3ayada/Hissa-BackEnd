using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Commands;

public record DeleteUnavailableSlotCommand(Guid SlotId) : IRequest<ApiResponse>;
