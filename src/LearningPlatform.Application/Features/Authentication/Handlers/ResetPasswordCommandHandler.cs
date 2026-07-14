using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Authentication.Handlers;

public class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ResetPasswordCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new BadRequestException("Invalid request.");

        string decodedToken;
        try
        {
            decodedToken = Uri.UnescapeDataString(request.Token);
        }
        catch (Exception)
        {
            throw new BadRequestException("Invalid or malformed reset token.");
        }

        var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));

        return ApiResponse.Success("Password has been reset successfully.");
    }
}
