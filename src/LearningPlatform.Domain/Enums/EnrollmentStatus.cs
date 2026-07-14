namespace LearningPlatform.Domain.Enums;

public enum EnrollmentStatus
{
    Active = 1,
    Completed = 2,
    Cancelled = 3,
    Expired = 4,

    /// <summary>Enrollment created but the manual payment has not yet been approved.</summary>
    PendingPayment = 5
}
