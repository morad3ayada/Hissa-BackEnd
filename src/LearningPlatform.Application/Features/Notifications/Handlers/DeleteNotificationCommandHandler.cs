using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Notifications.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Notifications.Handlers;

public class DeleteNotificationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<DeleteNotificationCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var notification = await unitOfWork.Repository<Notification>().GetByIdAsync(request.Id, cancellationToken);

        if (notification is null || notification.UserId != userId)
            throw new NotFoundException(nameof(Notification), request.Id);

        unitOfWork.Repository<Notification>().Remove(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Notification deleted.");
    }
}
