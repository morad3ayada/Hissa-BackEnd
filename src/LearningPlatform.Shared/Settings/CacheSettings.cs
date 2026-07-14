namespace LearningPlatform.Shared.Settings;

public class CacheSettings
{
    public const string SectionName = "CacheSettings";

    public bool UseDistributedCache { get; set; }
    public string? RedisConnectionString { get; set; }
    public int DefaultExpirationMinutes { get; set; } = 30;
}
