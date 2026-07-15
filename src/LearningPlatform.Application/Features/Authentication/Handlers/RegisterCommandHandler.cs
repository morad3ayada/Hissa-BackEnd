using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Application.Features.Authentication.DTOs;
using LearningPlatform.Application.Features.Authentication.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Authentication.Handlers;

public class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IMapper mapper)
    : IRequestHandler<RegisterCommand, ApiResponse<AuthResponseDto>>
{
    public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            throw new ConflictException("An account with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            IsActive = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            throw new BadRequestException(string.Join(" ", createResult.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, request.Role.ToString());

        await SendEmailConfirmationAsync(user, cancellationToken);

        var accessToken = jwtTokenService.GenerateAccessToken(user, [request.Role.ToString()]);
        var refreshToken = jwtTokenService.GenerateRefreshToken();

        await unitOfWork.Repository<RefreshToken>().AddAsync(new RefreshToken
        {
            Token = refreshToken.Value,
            UserId = user.Id,
            ExpiresAt = refreshToken.ExpiresAt
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<AuthResponseDto>(user);
        response.AccessToken = accessToken.Value;
        response.RefreshToken = refreshToken.Value;
        response.ExpiresAt = accessToken.ExpiresAt;

        return ApiResponse<AuthResponseDto>.Success(response, "Registration successful.");
    }

    private async Task SendEmailConfirmationAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        await emailService.SendAsync(new EmailMessage(
            user.Email!,
            "Confirm your email",
            $"Welcome to Learning Platform! Confirm your email using:\n" +
            $"GET /api/v1/Auth/confirm-email?userId={user.Id}&token={encodedToken}"), cancellationToken);
    }
}
