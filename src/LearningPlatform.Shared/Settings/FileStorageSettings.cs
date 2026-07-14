namespace LearningPlatform.Shared.Settings;

public class FileStorageSettings
{
    public const string SectionName = "FileStorageSettings";

    /// <summary>
    /// Root directory (relative to the app's content root, or absolute) under which
    /// every relative path passed to IFileStorageService is resolved, e.g. a call with
    /// fileName "PrivateVideos/Course/Section/LessonVideo.mp4" is stored at
    /// {RootPath}/PrivateVideos/Course/Section/LessonVideo.mp4.
    /// </summary>
    public string RootPath { get; set; } = "AppData";

    // Left empty (not defaulted) deliberately: the .NET configuration binder appends to,
    // rather than replaces, array-typed option properties that already hold a value,
    // which would duplicate every entry configured in appsettings.json.
    public string[] AllowedVideoExtensions { get; set; } = [];
    public string[] AllowedImageExtensions { get; set; } = [];
    public long MaxVideoSizeInBytes { get; set; } = 2L * 1024 * 1024 * 1024; // 2 GB
    public long MaxImageSizeInBytes { get; set; } = 5 * 1024 * 1024; // 5 MB
}
