using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Authentication.Interfaces;

public interface IJwtTokenService
{
    JwtToken GenerateAccessToken(ApplicationUser user, IList<string> roles);

    JwtToken GenerateRefreshToken();
}

public record JwtToken(string Value, DateTime ExpiresAt);
