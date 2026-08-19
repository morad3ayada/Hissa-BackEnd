using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class DeleteUnavailableSlotCommandHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Commands.DeleteUnavailableSlotCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(
        Commands.DeleteUnavailableSlotCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var slot = await unitOfWork.Repository<TeacherUnavailableSlot>()
            .GetByIdAsync(request.SlotId, cancellationToken)
            ?? throw new NotFoundException("Unavailable slot not found.");

        if (slot.TeacherId != userId)
            throw new ForbiddenException("You can only delete your own unavailable slots.");

        unitOfWork.Repository<TeacherUnavailableSlot>().Delete(slot);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Unavailable slot deleted successfully.");
    }
}
