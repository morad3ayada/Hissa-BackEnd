using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Subscriptions.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subscriptions.Handlers;

public class CancelSubscriptionCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<CancelSubscriptionCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var subscription = await unitOfWork.Repository<InstructorSubscription>()
            .AsQueryable()
            .Where(s => s.InstructorId == userId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("No active subscription found.");

        subscription.Status = SubscriptionStatus.Cancelled;
        unitOfWork.Repository<InstructorSubscription>().Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Subscription cancelled successfully.");
    }
}
