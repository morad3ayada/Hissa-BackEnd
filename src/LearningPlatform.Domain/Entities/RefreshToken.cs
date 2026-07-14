using System.ComponentModel.DataAnnotations.Schema;
using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string? CreatedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    [NotMapped]
    public bool IsRevoked => RevokedAt is not null;

    [NotMapped]
    public bool IsActive => !IsRevoked && !IsExpired;
}
