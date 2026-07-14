using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Shared.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LearningPlatform.Infrastructure.FileStorage;

/// <summary>
/// Stores files on local disk under {ContentRoot}/{FileStorageSettings.RootPath}/{fileName}.
/// The fileName is treated as a relative path, so callers control the folder structure
/// (e.g. "PrivateVideos/CourseName/SectionName/LessonVideo.mp4"); intermediate directories
/// are created automatically. Swap for a cloud-backed implementation later without touching callers.
/// </summary>
public class LocalFileStorageService(IHostEnvironment environment, IOptions<FileStorageSettings> settings)
    : IFileStorageService
{
    private readonly FileStorageSettings _settings = settings.Value;

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var fullPath = ResolvePath(fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var destination = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(destination, cancellationToken);

        return NormalizeRelativePath(fileName);
    }

    public Task<Stream> DownloadAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var fullPath = ResolvePath(fileUrl);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("The requested file was not found.", fileUrl);

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var fullPath = ResolvePath(fileUrl);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    private string ResolvePath(string relativePath)
    {
        var normalized = NormalizeRelativePath(relativePath);
        var root = Path.IsPathRooted(_settings.RootPath)
            ? _settings.RootPath
            : Path.Combine(environment.ContentRootPath, _settings.RootPath);

        var fullPath = Path.GetFullPath(Path.Combine(root, normalized));
        var fullRoot = Path.GetFullPath(root);

        if (!fullPath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Resolved path escapes the storage root.");

        return fullPath;
    }

    private static string NormalizeRelativePath(string relativePath) =>
        relativePath.Replace('\\', '/').TrimStart('/');
}
