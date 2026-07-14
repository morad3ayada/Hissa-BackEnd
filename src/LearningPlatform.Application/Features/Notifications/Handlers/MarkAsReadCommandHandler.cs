using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Notifications.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Notifications.Handlers;

public class MarkAsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<MarkAsReadCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var notification = await unitOfWork.Repository<Notification>().GetByIdAsync(request.Id, cancellationToken);

        if (notification is null || notification.UserId != userId)
            throw new NotFoundException(nameof(Notification), request.Id);

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            unitOfWork.Repository<Notification>().Update(notification);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return ApiResponse.Success("Notification marked as read.");
    }
}
