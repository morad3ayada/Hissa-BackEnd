using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Subscriptions.Commands;
using LearningPlatform.Application.Features.Subscriptions.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subscriptions.Handlers;

public class RenewSubscriptionCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<RenewSubscriptionCommand, ApiResponse<InstructorSubscriptionDto>>
{
    public async Task<ApiResponse<InstructorSubscriptionDto>> Handle(
        RenewSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var lastSubscription = await unitOfWork.Repository<InstructorSubscription>()
            .AsQueryable()
            .Include(s => s.Instructor)
            .Include(s => s.Plan)
            .Where(s => s.InstructorId == userId)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastSubscription is null)
            throw new BadRequestException("You have no previous subscription. Please subscribe first.");

        var now = DateTime.UtcNow;
        var newStartDate = lastSubscription.EndDate > now ? lastSubscription.EndDate : now;

        var subscription = new InstructorSubscription
        {
            InstructorId = userId,
            PlanId = lastSubscription.PlanId,
            StartDate = newStartDate,
            EndDate = newStartDate.AddDays(lastSubscription.Plan.DurationInDays),
            Status = SubscriptionStatus.Active,
            PaymentReference = request.PaymentReference
        };

        await unitOfWork.Repository<InstructorSubscription>().AddAsync(subscription, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new InstructorSubscriptionDto
        {
            Id = subscription.Id,
            InstructorId = subscription.InstructorId,
            PlanId = subscription.PlanId,
            PlanName = lastSubscription.Plan.Name,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            Status = subscription.Status
        };

        return ApiResponse<InstructorSubscriptionDto>.Success(dto, "Subscription renewed successfully.");
    }
}
