using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class UploadChatAttachmentCommandHandler(
    ICurrentUserService currentUser,
    IFileStorageService fileStorageService)
    : IRequestHandler<UploadChatAttachmentCommand, ApiResponse<UploadedAttachmentDto>>
{
    private const long MaxMediaSizeInBytes = 25L * 1024 * 1024;
    private const long MaxVideoSizeInBytes = 100L * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp",
        ".mp4", ".mov", ".avi", ".mkv", ".webm",
        ".mp3", ".wav", ".m4a", ".aac", ".ogg",
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".zip"
    ];

    public async Task<ApiResponse<UploadedAttachmentDto>> Handle(UploadChatAttachmentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        if (request.FileSize <= 0)
            throw new BadRequestException("The file is empty.");

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            throw new BadRequestException($"File type '{extension}' is not allowed for chat attachments.");

        var attachmentType = ResolveAttachmentType(extension, request.ContentType);

        var maxSize = attachmentType == AttachmentType.Video ? MaxVideoSizeInBytes : MaxMediaSizeInBytes;

        if (request.FileSize > maxSize)
            throw new BadRequestException($"The file exceeds the maximum allowed size of {maxSize / (1024 * 1024)} MB.");

        var relativePath = $"ChatAttachments/{userId}/{Guid.NewGuid():N}{extension}";

        var storedPath = await fileStorageService.UploadAsync(
            request.FileStream, relativePath, request.ContentType, cancellationToken);

        return ApiResponse<UploadedAttachmentDto>.Success(new UploadedAttachmentDto
        {
            FileName = Path.GetFileName(request.FileName),
            FileUrl = storedPath,
            ContentType = request.ContentType,
            FileSize = request.FileSize,
            AttachmentType = attachmentType.ToString()
        }, "Attachment uploaded.");
    }

    private static AttachmentType ResolveAttachmentType(string extension, string contentType)
    {
        if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return AttachmentType.Image;
        if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
            return AttachmentType.Video;
        if (contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
            return AttachmentType.Audio;

        return extension switch
        {
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".bmp" => AttachmentType.Image,
            ".mp4" or ".mov" or ".avi" or ".mkv" or ".webm" => AttachmentType.Video,
            ".mp3" or ".wav" or ".m4a" or ".aac" or ".ogg" => AttachmentType.Audio,
            _ => AttachmentType.File
        };
    }
}
