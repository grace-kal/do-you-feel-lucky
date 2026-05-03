using DoYouFeelLucky.App.Common;
using DoYouFeelLucky.App.Configuration;
using DoYouFeelLucky.App.Interfaces;
using DoYouFeelLucky.App.Models;
using DoYouFeelLucky.Wallet.Interfaces;
using Microsoft.Extensions.Options;

namespace DoYouFeelLucky.App.Services;

public class GameService(IWalletService walletService, IRngService rngService, IOptions<GameSettings> optionSettings) : IGameService
{
    private readonly GameSettings settings = optionSettings.Value;

    public async Task<BetResult> PlaceBetAsync(Guid walletId, Guid correlationId, decimal amount)
    {
        var debitResult = await walletService.WithdrawAsync(walletId, correlationId, Guid.NewGuid(), amount);
        if (!debitResult.Success) return Failure(debitResult.ErrorMessage!);

        var roll = rngService.Roll();
        var outcome = DetermineOutcome(roll);
        var multiplier = CalculateMultiplier(outcome);
        var winAmount = Math.Round(amount * multiplier, 2, MidpointRounding.ToEven);

        if (winAmount > 0)
        {
            var creditResult = await walletService.DepositAsync(walletId, correlationId, Guid.NewGuid(), winAmount);

            if (!creditResult.Success)
            {
                // TODO: rollback debit
                return Failure(creditResult.ErrorMessage!);
            }

            return Success(outcome, amount, winAmount, multiplier, creditResult.NewBalance);
        }

        return Success(outcome, amount, 0, 0, debitResult.NewBalance);
    }

    private OutcomeType DetermineOutcome(double roll)
    {
        if (roll < settings.LossProbability)
            return OutcomeType.Loss;

        if (roll < settings.LossProbability + settings.SmallWinProbability)
            return OutcomeType.SmallWin;

        return OutcomeType.BigWin;
    }

    private decimal CalculateMultiplier(OutcomeType outcome) => outcome switch
    {
        OutcomeType.Loss => 0,
        OutcomeType.SmallWin => rngService.GetRandomDecimal(settings.SmallWinMinMultiplier, settings.SmallWinMaxMultiplier),
        OutcomeType.BigWin => rngService.GetRandomDecimal(settings.BigWinMinMultiplier, settings.BigWinMaxMultiplier),
        _ => throw new ArgumentOutOfRangeException(nameof(outcome))
    };

    private static BetResult Failure(string errorMessage) =>
        new() { Success = false, ErrorMessage = errorMessage };

    private static BetResult Success(OutcomeType outcome, decimal betAmount, decimal winAmount, decimal multiplier, decimal newBalance) =>
        new()
        {
            Success = true,
            Outcome = outcome,
            BetAmount = betAmount,
            WinAmount = winAmount,
            Multiplier = multiplier,
            NewBalance = newBalance
        };
}