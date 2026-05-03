using DoYouFeelLucky.App.Constants;
using DoYouFeelLucky.App.Interfaces;
using DoYouFeelLucky.App.Models;
using DoYouFeelLucky.Common.Extensions;

namespace DoYouFeelLucky.App.Commands;

public class BetCommand(IGameService gameService, Guid walletId, decimal amount) : ICommand
{
    public async Task<string> ExecuteAsync()
    {
        var correlationId = Guid.NewGuid();

        var result = await gameService.PlaceBetAsync(walletId, correlationId, amount);

        if (!result.Success)
            return string.Format(Messages.Game.BetFailed, result.ErrorMessage);

        return result.Outcome == OutcomeType.Loss
           ? string.Format(Messages.Game.LossMessage, result.Outcome.GetDescription(), result.NewBalance)
           : string.Format(Messages.Game.WinMessage, result.Outcome.GetDescription(), result.WinAmount, result.NewBalance);
    }
}