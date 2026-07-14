using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Handlers;

public class LogoutCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<RefreshToken>();

        var tokens = await repository.FindAsync(t => t.Token == request.RefreshToken, cancellationToken);
        var token = tokens.FirstOrDefault();

        if (token is not null && token.IsActive)
        {
            token.RevokedAt = DateTime.UtcNow;
            repository.Update(token);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return ApiResponse.Success("Logged out successfully.");
    }
}
