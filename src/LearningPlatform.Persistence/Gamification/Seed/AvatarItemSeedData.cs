namespace LearningPlatform.Persistence.Gamification.Seed;

/// <summary>Fixed, deterministic Avatar Store catalog rows. "Base" category holds the two
/// free character choices (Boy/Girl); every other category is a purchasable customization.
/// See GamificationLevelSeedData for why anonymous objects are used instead of AvatarItem instances.</summary>
public static class AvatarItemSeedData
{
    private static readonly DateTime SeededAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static object[] Items =>
    [
        Build("c0000000-0000-0000-0000-000000000001", "Boy Avatar", "Base", "https://i.ibb.co/s9MYjpp8/image.png", 0, true),
        Build("c0000000-0000-0000-0000-000000000002", "Girl Avatar", "Base", "https://i.ibb.co/mV5hyVVP/image.png", 0, true),

        Build("c0000000-0000-0000-0000-000000000003", "Short Hair", "Hair", "https://i.ibb.co/7dXKTqvn/image.png", 20, false),
        Build("c0000000-0000-0000-0000-000000000004", "Long Hair", "Hair", "https://i.ibb.co/93YVcFhc/image.png", 20, false),
        Build("c0000000-0000-0000-0000-000000000005", "Curly Hair", "Hair", "https://i.ibb.co/fd7MBrjZ/image.png", 30, false),

        Build("c0000000-0000-0000-0000-000000000006", "T-Shirt", "Clothes", "https://i.ibb.co/chJTNqW2/image.png", 20, false),
        Build("c0000000-0000-0000-0000-000000000007", "Hoodie", "Clothes", "https://i.ibb.co/hGBqw7S/image.png", 35, false),
        Build("c0000000-0000-0000-0000-000000000008", "Suit", "Clothes", "https://i.ibb.co/cKwbQGjw/image.png", 60, false),

        Build("c0000000-0000-0000-0000-000000000009", "Round Glasses", "Glasses", "https://i.ibb.co/5XtVbtTc/image.png", 15, false),
        Build("c0000000-0000-0000-0000-00000000000a", "Sunglasses", "Glasses", "https://i.ibb.co/TDnmsf06/image.png", 25, false),

        Build("c0000000-0000-0000-0000-00000000000b", "Cap", "Hats", "https://i.ibb.co/DgDg4P0M/image.png", 15, false),
        Build("c0000000-0000-0000-0000-00000000000c", "Wizard Hat", "Hats", "https://i.ibb.co/Hf03zhMf/image.png", 40, false),

        Build("c0000000-0000-0000-0000-00000000000d", "Necklace", "Accessories", "https://i.ibb.co/Pz5kYSy2/image.png", 20, false),
        Build("c0000000-0000-0000-0000-00000000000e", "Watch", "Accessories", "https://i.ibb.co/7x7fBJt3/image.png", 30, false)
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
