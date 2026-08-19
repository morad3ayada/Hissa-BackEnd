using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class InstructorSubscription : BaseEntity
{
    public Guid InstructorId { get; set; }
    public ApplicationUser Instructor { get; set; } = null!;

    public Guid PlanId { get; set; }
    public SubscriptionPlan Plan { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public string? PaymentReference { get; set; }
}
