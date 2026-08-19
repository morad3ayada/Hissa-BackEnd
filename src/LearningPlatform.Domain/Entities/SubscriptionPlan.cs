using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class SubscriptionPlan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationInDays { get; set; }
    public int MaxCourses { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<InstructorSubscription> Subscriptions { get; set; } = [];
}
