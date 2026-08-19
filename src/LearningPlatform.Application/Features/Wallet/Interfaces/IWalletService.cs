namespace LearningPlatform.Application.Features.Wallet.Interfaces;

public interface IWalletService
{
    /// <summary>Returns the student's wallet, lazily creating it (balance 0) on first use.</summary>
    Task<LearningPlatform.Domain.Entities.Wallet> GetOrCreateAsync(
        Guid studentId,
        CancellationToken cancellationToken = default);
}
