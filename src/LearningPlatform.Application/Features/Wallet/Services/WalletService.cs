using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Wallet.Interfaces;

namespace LearningPlatform.Application.Features.Wallet.Services;

public class WalletService(IUnitOfWork unitOfWork) : IWalletService
{
    public async Task<LearningPlatform.Domain.Entities.Wallet> GetOrCreateAsync(
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var repository = unitOfWork.Repository<LearningPlatform.Domain.Entities.Wallet>();

        var wallet = await repository.GetTrackedAsync(w => w.StudentId == studentId, cancellationToken);
        if (wallet is not null)
            return wallet;

        wallet = new LearningPlatform.Domain.Entities.Wallet { StudentId = studentId, Balance = 0 };
        await repository.AddAsync(wallet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return wallet;
    }
}
