using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Subscriptions.DTOs;
using LearningPlatform.Application.Features.Subscriptions.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subscriptions.Handlers;

public class GetSubscriptionPlansQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSubscriptionPlansQuery, ApiResponse<List<SubscriptionPlanDto>>>
{
    public async Task<ApiResponse<List<SubscriptionPlanDto>>> Handle(
        GetSubscriptionPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await unitOfWork.Repository<SubscriptionPlan>()
            .AsQueryable()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Price)
            .Select(p => new SubscriptionPlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DurationInDays = p.DurationInDays,
                MaxCourses = p.MaxCourses,
                IsActive = p.IsActive
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<SubscriptionPlanDto>>.Success(plans);
    }
}
