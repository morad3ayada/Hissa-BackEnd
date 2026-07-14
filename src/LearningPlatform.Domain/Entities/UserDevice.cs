using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class UserDevice : BaseEntity
{
    public string DeviceToken { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; }
    public string? DeviceName { get; set; }
    public DateTime LastActiveAt { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}
