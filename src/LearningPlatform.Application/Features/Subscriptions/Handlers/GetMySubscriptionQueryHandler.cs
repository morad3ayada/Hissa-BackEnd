using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Subscriptions.DTOs;
using LearningPlatform.Application.Features.Subscriptions.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subscriptions.Handlers;

public class GetMySubscriptionQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<GetMySubscriptionQuery, ApiResponse<InstructorSubscriptionDto?>>
{
    public async Task<ApiResponse<InstructorSubscriptionDto?>> Handle(
        GetMySubscriptionQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var subscription = await unitOfWork.Repository<InstructorSubscription>()
            .AsQueryable()
            .Include(s => s.Instructor)
            .Include(s => s.Plan)
            .Where(s => s.InstructorId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (subscription is null)
            return ApiResponse<InstructorSubscriptionDto?>.Success(null, "No subscription found.");

        var dto = new InstructorSubscriptionDto
        {
            Id = subscription.Id,
            InstructorId = subscription.InstructorId,
            InstructorName = $"{subscription.Instructor.FirstName} {subscription.Instructor.LastName}",
            PlanId = subscription.PlanId,
            PlanName = subscription.Plan.Name,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            Status = subscription.Status
        };

        return ApiResponse<InstructorSubscriptionDto?>.Success(dto);
    }
}
