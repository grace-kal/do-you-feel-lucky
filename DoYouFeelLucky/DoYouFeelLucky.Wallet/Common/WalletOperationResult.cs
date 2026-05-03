using DoYouFeelLucky.Common.Models;

namespace DoYouFeelLucky.Wallet.Common;

public class WalletOperationResult : BaseResult
{
    public decimal NewBalance { get; init; }
}