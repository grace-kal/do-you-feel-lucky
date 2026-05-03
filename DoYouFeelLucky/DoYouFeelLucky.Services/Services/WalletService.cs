using DoYouFeelLucky.Wallet.Common;
using DoYouFeelLucky.Wallet.Interfaces;
using DoYouFeelLucky.Wallet.Models;

namespace DoYouFeelLucky.Wallet.Services;

public class WalletService : IWalletService
{
    private readonly PlayerWallet _wallet;

    public WalletService() => _wallet = new PlayerWallet { Id = Guid.NewGuid() };

    public Task<WalletOperationResult> DepositAsync(Guid walletId, Guid correlationId, Guid idempotencyKey, decimal amount)
    {
        if (amount <= 0)
            return Task.FromResult(Failure("Deposit amount must be positive."));

        return ProcessTransactionAsync(TransactionType.Deposit, correlationId, idempotencyKey, amount);
    }

    public Task<WalletOperationResult> WithdrawAsync(Guid walletId, Guid correlationId, Guid idempotencyKey, decimal amount)
    {
        if (amount <= 0)
            return Task.FromResult(Failure("Withdrawal amount must be positive."));

        if (amount > _wallet.Balance)
            return Task.FromResult(Failure("Insufficient funds."));

        return ProcessTransactionAsync(TransactionType.Withdrawal, correlationId, idempotencyKey, amount);
    }

    private Task<WalletOperationResult> ProcessTransactionAsync(TransactionType type, Guid correlationId, Guid idempotencyKey, decimal amount)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            CorrelationId = correlationId,
            IdempotencyKey = idempotencyKey,
            Type = type,
            Amount = amount,
            BalanceBefore = _wallet.Balance,
            Status = TransactionStatus.Pending,
            Timestamp = DateTime.UtcNow
        };

        _wallet.AddTransaction(transaction);
        transaction.Status = TransactionStatus.Completed;
        transaction.BalanceAfter = _wallet.Balance;

        return Task.FromResult(Succeeded(_wallet.Balance));
    }

    private static WalletOperationResult Failure(string errorMessage) =>
        new() { Success = false, ErrorMessage = errorMessage };

    private static WalletOperationResult Succeeded(decimal newBalance) =>
        new() { Success = true, NewBalance = newBalance };
}