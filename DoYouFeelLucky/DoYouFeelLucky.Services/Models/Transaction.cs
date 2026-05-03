namespace DoYouFeelLucky.Wallet.Models;

public class Transaction
{
    public Guid Id { get; init; }
    public Guid CorrelationId { get; init; }
    public Guid IdempotencyKey { get; init; }

    // Always stored as a positive value as per task requirements
    public decimal Amount { get; init; }
    public decimal BalanceBefore { get; init; }
    public decimal BalanceAfter { get; set; }
    public TransactionStatus Status { get; set; }
    public TransactionType Type { get; init; }
    public DateTime Timestamp { get; init; }
}