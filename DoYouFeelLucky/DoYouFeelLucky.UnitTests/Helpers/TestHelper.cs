using DoYouFeelLucky.App.Configuration;

namespace DoYouFeelLucky.UnitTests.Helpers;

public static class TestHelper
{
    public static GameSettings GameSettings => new()
    {
        MinBet = 1,
        MaxBet = 10,
        LossProbability = 0.50,
        SmallWinProbability = 0.40,
        SmallWinMinMultiplier = 1,
        SmallWinMaxMultiplier = 2,
        BigWinMinMultiplier = 2,
        BigWinMaxMultiplier = 10
    };
}