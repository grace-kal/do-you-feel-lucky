using DoYouFeelLucky.App.Constants;
using DoYouFeelLucky.Wallet.Interfaces;

namespace DoYouFeelLucky.App.Commands;

public class DepositCommand(IWalletService walletService, Guid walletId, decimal amount) : ICommand
{
    public async Task<string> ExecuteAsync()
    {
        var correlationId = Guid.NewGuid();
        var idempotencyKey = Guid.NewGuid();

        var result = await walletService.DepositAsync(walletId, correlationId, idempotencyKey, amount);

        return result.Success
            ? string.Format(Messages.Wallet.DepositSuccess, amount, result.NewBalance)
            : string.Format(Messages.Wallet.DepositFailed, result.ErrorMessage);
    }
}
