namespace LearningPlatform.Persistence.Gamification.Seed;

/// <summary>
/// Fixed, deterministic level-threshold rows for EF Core migration HasData seeding.
/// Anonymous objects (not GamificationLevel instances) are required because BaseEntity.Id
/// has a protected setter that this assembly cannot assign through an object initializer;
/// HasData matches anonymous-object properties to the entity's shadow model by name instead.
/// Ids must never change once a migration referencing them has shipped.
/// </summary>
public static class GamificationLevelSeedData
{
    private static readonly DateTime SeededAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static object[] Levels =>
    [
        Build("a0000000-0000-0000-0000-000000000001", 1, "Beginner", 0),
        Build("a0000000-0000-0000-0000-000000000002", 2, "Learner", 100),
        Build("a0000000-0000-0000-0000-000000000003", 3, "Achiever", 300),
        Build("a0000000-0000-0000-0000-000000000004", 4, "Expert", 600),
        Build("a0000000-0000-0000-0000-000000000005", 5, "Master", 1000),
        Build("a0000000-0000-0000-0000-000000000006", 6, "Legend", 2000)
    ];

    private static object Build(string id, int levelNumber, string title, int requiredPoints) => new
    {
        Id = Guid.Parse(id),
        LevelNumber = levelNumber,
        Title = title,
        RequiredPoints = requiredPoints,
        CreatedAt = SeededAt,
        UpdatedAt = (DateTime?)null,
        IsDeleted = false
    };
}
