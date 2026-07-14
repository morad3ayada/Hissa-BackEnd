using System.Text;
using System.Text.RegularExpressions;

namespace LearningPlatform.Shared.Helpers;

public static partial class SlugGenerator
{
    public static string GenerateSlug(string title)
    {
        var slug = title.Trim().ToLowerInvariant();
        slug = DiacriticsRegex().Replace(slug, "");
        slug = InvalidCharsRegex().Replace(slug, "-");
        slug = MultipleHyphensRegex().Replace(slug, "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug)
            ? Guid.NewGuid().ToString("N")[..8]
            : slug;
    }

    public static string GenerateUniqueSlug(string title) =>
        $"{GenerateSlug(title)}-{Guid.NewGuid().ToString("N")[..6]}";

    /// <summary>
    /// Sanitizes a title for safe use as a filesystem folder name
    /// (e.g. building the PrivateVideos/CourseName/SectionName path).
    /// </summary>
    public static string ToSafeFolderName(string value)
    {
        var builder = new StringBuilder(value.Length);

        foreach (var c in value)
        {
            builder.Append(Array.IndexOf(Path.GetInvalidFileNameChars(), c) >= 0 ? '-' : c);
        }

        var sanitized = MultipleHyphensRegex().Replace(builder.ToString(), "-").Trim('-', ' ');

        return string.IsNullOrWhiteSpace(sanitized) ? Guid.NewGuid().ToString("N")[..8] : sanitized;
    }

    [GeneratedRegex(@"\p{Mn}")]
    private static partial Regex DiacriticsRegex();

    [GeneratedRegex(@"[^a-z0-9؀-ۿ]+")]
    private static partial Regex InvalidCharsRegex();

    [GeneratedRegex(@"-+")]
    private static partial Regex MultipleHyphensRegex();
}
