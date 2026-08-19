using LearningPlatform.Application.Common.Helpers;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Authentication.Handlers;

public class VerifyOtpCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<VerifyOtpCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = userManager.NormalizeEmail(request.Email);

        var repository = unitOfWork.Repository<EmailOtp>();

        var otpRecord = await repository.AsQueryable()
            .Where(o => o.Email == normalizedEmail && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otpRecord is null)
            return ApiResponse.Fail("Invalid or expired OTP.");

        var now = dateTimeProvider.UtcNow;

        if (otpRecord.ExpiresAt < now)
            return ApiResponse.Fail("Invalid or expired OTP.");

        if (otpRecord.Attempts >= otpRecord.MaxAttempts)
            return ApiResponse.Fail("Maximum verification attempts exceeded. Please request a new code.");

        otpRecord.Attempts++;

        var inputHash = OtpHasher.Hash(request.Otp);

        if (otpRecord.OtpHash != inputHash)
        {
            repository.Update(otpRecord);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return ApiResponse.Fail("Invalid or expired OTP.");
        }

        otpRecord.IsUsed = true;
        repository.Update(otpRecord);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is not null && !user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Email verified successfully.");
    }
}
