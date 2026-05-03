namespace DoYouFeelLucky.Wallet.Models;

public class PlayerWallet
{
    public Guid Id { get; init; }

    // The list and AddTransaction method below are in memory substitutes for the purpose of this task
    private readonly List<Transaction> _transactions = new();
    public IReadOnlyList<Transaction> Transactions => _transactions;

    public decimal Balance => Math.Round(_transactions
    .Where(t => t.Status == TransactionStatus.Completed)
    .Sum(t => t.Type == TransactionType.Deposit
        ? t.Amount
        : -t.Amount), 2);

    public void AddTransaction(Transaction transaction)
    {
        _transactions.Add(transaction);
    }
}