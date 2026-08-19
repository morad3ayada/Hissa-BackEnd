namespace LearningPlatform.Application.Features.Wallet.DTOs;

public class WalletSummaryDto
{
    public Guid WalletId { get; set; }
    public decimal Balance { get; set; }
    public int TotalTransactionsCount { get; set; }
    public decimal TotalDeposited { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public int WithdrawalsThisMonthCount { get; set; }
    public decimal WithdrawalsThisMonthAmount { get; set; }
    public int PendingDepositRequests { get; set; }
    public int PendingWithdrawalRequests { get; set; }
}
