using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Application.Features.Authentication.DTOs;
using LearningPlatform.Application.Features.Authentication.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Authentication.Handlers;

public class RefreshTokenCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponseDto>>
{
    public async Task<ApiResponse<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<RefreshToken>();

        var existingTokens = await repository.FindAsync(t => t.Token == request.RefreshToken, cancellationToken);
        var existingToken = existingTokens.FirstOrDefault()
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!existingToken.IsActive)
            throw new UnauthorizedException("This refresh token is no longer valid. Please sign in again.");

        var user = await userManager.FindByIdAsync(existingToken.UserId.ToString())
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!user.IsActive)
            throw new ForbiddenException("This account has been deactivated.");

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtTokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = jwtTokenService.GenerateRefreshToken();

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByToken = newRefreshToken.Value;
        repository.Update(existingToken);

        await repository.AddAsync(new RefreshToken
        {
            Token = newRefreshToken.Value,
            UserId = user.Id,
            ExpiresAt = newRefreshToken.ExpiresAt
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<AuthResponseDto>(user);
        response.AccessToken = accessToken.Value;
        response.RefreshToken = newRefreshToken.Value;
        response.ExpiresAt = accessToken.ExpiresAt;

        return ApiResponse<AuthResponseDto>.Success(response, "Token refreshed successfully.");
    }
}
