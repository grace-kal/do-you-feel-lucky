using DoYouFeelLucky.App.Common;

namespace DoYouFeelLucky.App.Interfaces;

public interface IGameService
{
    Task<BetResult> PlaceBetAsync(Guid walletId, Guid correlationId, decimal amount);
}