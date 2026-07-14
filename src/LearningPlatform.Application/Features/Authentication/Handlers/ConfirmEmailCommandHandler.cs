using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Authentication.Handlers;

public class ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ConfirmEmailCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString())
            ?? throw new NotFoundException(nameof(ApplicationUser), request.UserId);

        if (user.EmailConfirmed)
            return ApiResponse.Success("Email is already confirmed.");

        string decodedToken;
        try
        {
            decodedToken = Uri.UnescapeDataString(request.Token);
        }
        catch (Exception)
        {
            throw new BadRequestException("Invalid or malformed confirmation token.");
        }

        var result = await userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));

        return ApiResponse.Success("Email confirmed successfully.");
    }
}
