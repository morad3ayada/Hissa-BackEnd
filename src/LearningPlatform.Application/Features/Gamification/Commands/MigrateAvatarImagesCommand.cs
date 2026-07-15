using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Gamification.Commands;

public record MigrateAvatarImagesCommand : IRequest<ApiResponse<string>>;

public class MigrateAvatarImagesCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<MigrateAvatarImagesCommand, ApiResponse<string>>
{
    private static readonly Dictionary<string, string> ImageMap = new()
    {
        ["/avatars/base/boy.png"] = "https://i.ibb.co/s9MYjpp8/image.png",
        ["/avatars/base/girl.png"] = "https://i.ibb.co/mV5hyVVP/image.png",
        ["/avatars/hair/short.png"] = "https://i.ibb.co/7dXKTqvn/image.png",
        ["/avatars/hair/long.png"] = "https://i.ibb.co/93YVcFhc/image.png",
        ["/avatars/hair/curly.png"] = "https://i.ibb.co/fd7MBrjZ/image.png",
        ["/avatars/clothes/tshirt.png"] = "https://i.ibb.co/chJTNqW2/image.png",
        ["/avatars/clothes/hoodie.png"] = "https://i.ibb.co/hGBqw7S/image.png",
        ["/avatars/clothes/suit.png"] = "https://i.ibb.co/cKwbQGjw/image.png",
        ["/avatars/glasses/round.png"] = "https://i.ibb.co/5XtVbtTc/image.png",
        ["/avatars/glasses/sun.png"] = "https://i.ibb.co/TDnmsf06/image.png",
        ["/avatars/hats/cap.png"] = "https://i.ibb.co/DgDg4P0M/image.png",
        ["/avatars/hats/wizard.png"] = "https://i.ibb.co/Hf03zhMf/image.png",
        ["/avatars/accessories/necklace.png"] = "https://i.ibb.co/Pz5kYSy2/image.png",
        ["/avatars/accessories/watch.png"] = "https://i.ibb.co/7x7fBJt3/image.png",
    };

    public async Task<ApiResponse<string>> Handle(MigrateAvatarImagesCommand request, CancellationToken ct)
    {
        var repo = unitOfWork.Repository<AvatarItem>();
        var oldPaths = ImageMap.Keys.ToList();
        var items = await repo.FindAsync(i => oldPaths.Contains(i.ImageUrl), ct);

        int count = 0;
        foreach (var item in items)
        {
            if (ImageMap.TryGetValue(item.ImageUrl, out var newUrl))
            {
                item.ImageUrl = newUrl;
                item.UpdatedAt = DateTime.UtcNow;
                repo.Update(item);
                count++;
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        return ApiResponse<string>.Success($"Updated {count} avatar image(s).");
    }
}
