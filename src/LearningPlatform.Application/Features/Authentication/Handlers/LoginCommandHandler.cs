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

public class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork,
    IGamificationService gamificationService,
    IMapper mapper)
    : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new BadRequestException(InvalidCredentialsMessage);

        if (!user.IsActive)
            throw new ForbiddenException("This account has been deactivated.");

        if (await userManager.IsLockedOutAsync(user))
            throw new ForbiddenException("This account is temporarily locked due to multiple failed sign-in attempts.");

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user);
            throw new BadRequestException(InvalidCredentialsMessage);
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var roles = await userManager.GetRolesAsync(user);

        var accessToken = jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = jwtTokenService.GenerateRefreshToken();

        await unitOfWork.Repository<RefreshToken>().AddAsync(new RefreshToken
        {
            Token = refreshToken.Value,
            UserId = user.Id,
            ExpiresAt = refreshToken.ExpiresAt
        }, cancellationToken);

        if (user.Role == UserRole.Student)
            await gamificationService.TryAwardDailyLoginAsync(user.Id, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<AuthResponseDto>(user);
        response.AccessToken = accessToken.Value;
        response.RefreshToken = refreshToken.Value;
        response.ExpiresAt = accessToken.ExpiresAt;

        return ApiResponse<AuthResponseDto>.Success(response, "Login successful.");
    }
}
