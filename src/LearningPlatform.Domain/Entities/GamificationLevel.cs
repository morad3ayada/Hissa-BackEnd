using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

/// <summary>Admin-configurable level threshold lookup table (seeded); a student's
/// CurrentLevel is the highest LevelNumber whose RequiredPoints they've reached.</summary>
public class GamificationLevel : BaseEntity
{
    public int LevelNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public int RequiredPoints { get; set; }
}
