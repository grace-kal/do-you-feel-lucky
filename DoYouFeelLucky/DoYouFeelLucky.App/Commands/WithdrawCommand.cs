using DoYouFeelLucky.App.Constants;
using DoYouFeelLucky.Wallet.Interfaces;

namespace DoYouFeelLucky.App.Commands;

public class WithdrawCommand(IWalletService walletService, Guid walletId, decimal amount) : ICommand
{
    public async Task<string> ExecuteAsync()
    {
        var correlationId = Guid.NewGuid();
        var idempotencyKey = Guid.NewGuid();

        var result = await walletService.WithdrawAsync(walletId, correlationId, idempotencyKey, amount);

        return result.Success
            ? string.Format(Messages.Wallet.WithdrawSuccess, amount, result.NewBalance)
            : string.Format(Messages.Wallet.WithdrawFailed, result.ErrorMessage);
    }
}