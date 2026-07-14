namespace LearningPlatform.Persistence.Gamification.Seed;

/// <summary>Fixed, deterministic Avatar Store catalog rows. "Base" category holds the two
/// free character choices (Boy/Girl); every other category is a purchasable customization.
/// See GamificationLevelSeedData for why anonymous objects are used instead of AvatarItem instances.</summary>
public static class AvatarItemSeedData
{
    private static readonly DateTime SeededAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static object[] Items =>
    [
        Build("c0000000-0000-0000-0000-000000000001", "Boy Avatar", "Base", "/avatars/base/boy.png", 0, true),
        Build("c0000000-0000-0000-0000-000000000002", "Girl Avatar", "Base", "/avatars/base/girl.png", 0, true),

        Build("c0000000-0000-0000-0000-000000000003", "Short Hair", "Hair", "/avatars/hair/short.png", 20, false),
        Build("c0000000-0000-0000-0000-000000000004", "Long Hair", "Hair", "/avatars/hair/long.png", 20, false),
        Build("c0000000-0000-0000-0000-000000000005", "Curly Hair", "Hair", "/avatars/hair/curly.png", 30, false),

        Build("c0000000-0000-0000-0000-000000000006", "T-Shirt", "Clothes", "/avatars/clothes/tshirt.png", 20, false),
        Build("c0000000-0000-0000-0000-000000000007", "Hoodie", "Clothes", "/avatars/clothes/hoodie.png", 35, false),
        Build("c0000000-0000-0000-0000-000000000008", "Suit", "Clothes", "/avatars/clothes/suit.png", 60, false),

        Build("c0000000-0000-0000-0000-000000000009", "Round Glasses", "Glasses", "/avatars/glasses/round.png", 15, false),
        Build("c0000000-0000-0000-0000-00000000000a", "Sunglasses", "Glasses", "/avatars/glasses/sun.png", 25, false),

        Build("c0000000-0000-0000-0000-00000000000b", "Cap", "Hats", "/avatars/hats/cap.png", 15, false),
        Build("c0000000-0000-0000-0000-00000000000c", "Wizard Hat", "Hats", "/avatars/hats/wizard.png", 40, false),

        Build("c0000000-0000-0000-0000-00000000000d", "Necklace", "Accessories", "/avatars/accessories/necklace.png", 20, false),
        Build("c0000000-0000-0000-0000-00000000000e", "Watch", "Accessories", "/avatars/accessories/watch.png", 30, false)
    ];

    private static object Build(string id, string name, string category, string imageUrl, int priceInPoints, bool isDefault) => new
    {
        Id = Guid.Parse(id),
        Name = name,
        Category = category,
        ImageUrl = imageUrl,
        PriceInPoints = priceInPoints,
        IsDefault = isDefault,
        CreatedAt = SeededAt,
        UpdatedAt = (DateTime?)null,
        IsDeleted = false
    };
}
