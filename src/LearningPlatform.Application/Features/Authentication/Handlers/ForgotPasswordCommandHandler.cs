using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Authentication.Handlers;

public class ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService)
    : IRequestHandler<ForgotPasswordCommand, ApiResponse>
{
    private const string GenericSuccessMessage =
        "If an account with this email exists, a password reset link has been sent.";

    public async Task<ApiResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        // Always return the same generic response, whether the account exists or not,
        // so this endpoint can't be used to enumerate registered emails.
        if (user is null || !user.IsActive)
            return ApiResponse.Success(GenericSuccessMessage);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        await emailService.SendAsync(new EmailMessage(
            user.Email!,
            "Reset your password",
            $"Use this token to reset your password via POST /api/v1/Auth/reset-password:\n{encodedToken}"),
            cancellationToken);

        return ApiResponse.Success(GenericSuccessMessage);
    }
}
