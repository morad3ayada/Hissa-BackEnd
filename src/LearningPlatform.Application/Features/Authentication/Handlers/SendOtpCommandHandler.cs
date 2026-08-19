using System.Security.Cryptography;
using LearningPlatform.Application.Common.Helpers;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Authentication.Handlers;

public class SendOtpCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<SendOtpCommand, ApiResponse>
{
    private const int OtpLength = 6;
    private const int OtpExpiryMinutes = 5;
    private const int CooldownSeconds = 60;

    public async Task<ApiResponse> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = userManager.NormalizeEmail(request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive)
            return ApiResponse.Success("If an account with this email exists, an OTP has been sent.");

        var repository = unitOfWork.Repository<EmailOtp>();
        var now = dateTimeProvider.UtcNow;

        var lastOtp = await repository.AsQueryable()
            .Where(o => o.Email == normalizedEmail)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastOtp is not null && (now - lastOtp.CreatedAt).TotalSeconds < CooldownSeconds)
        {
            var waitSeconds = CooldownSeconds - (int)(now - lastOtp.CreatedAt).TotalSeconds;
            return ApiResponse.Fail($"Please wait {waitSeconds} seconds before requesting a new code.");
        }

        var otpCode = GenerateOtp();
        var otpHash = OtpHasher.Hash(otpCode);

        var otpEntity = new EmailOtp
        {
            Email = normalizedEmail,
            OtpHash = otpHash,
            ExpiresAt = now.AddMinutes(OtpExpiryMinutes),
            IsUsed = false,
            Attempts = 0,
            MaxAttempts = 5
        };

        await repository.AddAsync(otpEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var body = BuildEmailBody(otpCode);
        try
        {
            await emailService.SendAsync(
                new EmailMessage(request.Email, "Your Verification Code", body),
                cancellationToken);
        }
        catch (Exception)
        {
            return ApiResponse.Fail("Failed to send verification email. Please try again later.");
        }

        return ApiResponse.Success("OTP has been sent successfully.");
    }

    private static string GenerateOtp()
    {
        var bytes = RandomNumberGenerator.GetBytes(4);
        var number = BitConverter.ToUInt32(bytes) % 1_000_000;
        return number.ToString().PadLeft(OtpLength, '0');
    }

    private static string BuildEmailBody(string otp) =>
        """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8">
            <style>
                body { font-family: Arial, sans-serif; background: #f4f4f4; margin: 0; padding: 20px; }
                .container { max-width: 480px; margin: auto; background: #fff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
                .header { background: #4f46e5; color: #fff; padding: 24px; text-align: center; font-size: 20px; font-weight: bold; }
                .body { padding: 32px 24px; text-align: center; }
                .otp { font-size: 36px; font-weight: bold; letter-spacing: 8px; color: #4f46e5; margin: 24px 0; padding: 16px; background: #f0f0ff; border-radius: 8px; }
                .note { font-size: 14px; color: #666; margin-top: 24px; line-height: 1.6; }
            </style>
        </head>
        <body>
            <div class="container">
                <div class="header">Email Verification</div>
                <div class="body">
                    <p style="font-size:16px; color:#333;">Your verification code is:</p>
                    <div class="otp">OTP_PLACEHOLDER</div>
                    <p class="note">This code will expire in MINUTES_PLACEHOLDER minutes.</p>
                    <p class="note">If you did not request this code, please ignore this email.</p>
                </div>
            </div>
        </body>
        </html>
        """.Replace("OTP_PLACEHOLDER", otp).Replace("MINUTES_PLACEHOLDER", OtpExpiryMinutes.ToString());
}
