using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class EmailOtp : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string OtpHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public int Attempts { get; set; }
    public int MaxAttempts { get; set; } = 5;
}
