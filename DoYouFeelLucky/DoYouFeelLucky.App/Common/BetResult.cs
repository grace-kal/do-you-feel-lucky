using DoYouFeelLucky.App.Models;
using DoYouFeelLucky.Common.Models;

namespace DoYouFeelLucky.App.Common;

public class BetResult : BaseResult
{
    public OutcomeType Outcome { get; init; }
    public decimal BetAmount { get; init; }
    public decimal WinAmount { get; init; }
    public decimal Multiplier { get; init; }
    public decimal NewBalance { get; init; }
}