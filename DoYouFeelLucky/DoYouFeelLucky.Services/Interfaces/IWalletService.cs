using DoYouFeelLucky.Wallet.Common;

namespace DoYouFeelLucky.Wallet.Interfaces;

public interface IWalletService
{
    Task<WalletOperationResult> DepositAsync(Guid walletId, Guid correlationId, Guid idempotencyKey, decimal amount);
    Task<WalletOperationResult> WithdrawAsync(Guid walletId, Guid correlationId, Guid idempotencyKey, decimal amount);
}
