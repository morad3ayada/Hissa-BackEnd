using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class UserSettings : BaseEntity
{
    public string Language { get; set; } = "ar";
    public string Theme { get; set; } = "Light";
    public bool EmailNotificationsEnabled { get; set; } = true;
    public bool PushNotificationsEnabled { get; set; } = true;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}
