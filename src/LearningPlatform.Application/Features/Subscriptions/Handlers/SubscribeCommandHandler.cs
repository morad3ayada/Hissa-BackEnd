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

public class SubscribeCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<SubscribeCommand, ApiResponse<InstructorSubscriptionDto>>
{
    public async Task<ApiResponse<InstructorSubscriptionDto>> Handle(
        SubscribeCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var plan = await unitOfWork.Repository<SubscriptionPlan>()
            .GetByIdAsync(request.PlanId, cancellationToken)
            ?? throw new NotFoundException("Subscription plan not found.");

        if (!plan.IsActive)
            throw new BadRequestException("This subscription plan is no longer available.");

        var existingSubscription = await unitOfWork.Repository<InstructorSubscription>()
            .AsQueryable()
            .Include(s => s.Instructor)
            .Include(s => s.Plan)
            .Where(s => s.InstructorId == userId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSubscription is not null && existingSubscription.EndDate > DateTime.UtcNow)
            throw new BadRequestException("You already have an active subscription.");

        var now = DateTime.UtcNow;
        var subscription = new InstructorSubscription
        {
            InstructorId = userId,
            PlanId = request.PlanId,
            StartDate = now,
            EndDate = now.AddDays(plan.DurationInDays),
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
            PlanName = plan.Name,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            Status = subscription.Status
        };

        return ApiResponse<InstructorSubscriptionDto>.Success(dto, "Subscription created successfully.");
    }
}
