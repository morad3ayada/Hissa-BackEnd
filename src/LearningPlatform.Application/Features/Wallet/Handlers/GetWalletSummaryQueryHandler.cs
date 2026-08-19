using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Application.Features.Wallet.Interfaces;
using LearningPlatform.Application.Features.Wallet.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Wallet.Handlers;

public class GetWalletSummaryQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IWalletService walletService,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<GetWalletSummaryQuery, ApiResponse<WalletSummaryDto>>
{
    public async Task<ApiResponse<WalletSummaryDto>> Handle(GetWalletSummaryQuery request, CancellationToken cancellationToken)
    {
        var wallet = await walletService.GetOrCreateAsync(currentUser.UserId!.Value, cancellationToken);

        var transactions = unitOfWork.Repository<WalletTransaction>()
            .AsQueryable()
            .Where(t => t.WalletId == wallet.Id);

        var lifetimeTotals = await transactions
            .GroupBy(t => t.Type)
            .Select(g => new { Type = g.Key, Count = g.Count(), Total = g.Sum(t => t.Amount) })
            .ToListAsync(cancellationToken);

        var now = dateTimeProvider.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var withdrawalsThisMonth = await transactions
            .Where(t => t.Type == WalletTransactionType.Withdrawal && t.CreatedAt >= monthStart)
            .GroupBy(t => 1)
            .Select(g => new { Count = g.Count(), Total = g.Sum(t => t.Amount) })
            .FirstOrDefaultAsync(cancellationToken);

        var pending = await unitOfWork.Repository<WalletRequest>()
            .AsQueryable()
            .Where(r => r.WalletId == wallet.Id && r.Status == WalletRequestStatus.Pending)
            .GroupBy(r => r.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var summary = new WalletSummaryDto
        {
            WalletId = wallet.Id,
            Balance = wallet.Balance,
            TotalTransactionsCount = lifetimeTotals.Sum(x => x.Count),
            TotalDeposited = lifetimeTotals.Where(x => x.Type == WalletTransactionType.Deposit).Sum(x => x.Total),
            TotalWithdrawn = lifetimeTotals.Where(x => x.Type == WalletTransactionType.Withdrawal).Sum(x => x.Total),
            WithdrawalsThisMonthCount = withdrawalsThisMonth?.Count ?? 0,
            WithdrawalsThisMonthAmount = withdrawalsThisMonth?.Total ?? 0,
            PendingDepositRequests = pending.Where(x => x.Type == WalletRequestType.Deposit).Sum(x => x.Count),
            PendingWithdrawalRequests = pending.Where(x => x.Type == WalletRequestType.Withdrawal).Sum(x => x.Count)
        };

        return ApiResponse<WalletSummaryDto>.Success(summary);
    }
}
