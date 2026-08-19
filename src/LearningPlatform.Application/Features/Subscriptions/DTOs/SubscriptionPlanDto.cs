using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Subscriptions.DTOs;

public class SubscriptionPlanDto : IMapFrom<SubscriptionPlan>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationInDays { get; set; }
    public int MaxCourses { get; set; }
    public bool IsActive { get; set; }
}
