namespace DoYouFeelLucky.App.Configuration;

//In prod these settings would be loaded from a config file so we don't need to redeploy to change them
public class GameSettings
{
    public decimal MinBet { get; init; }
    public decimal MaxBet { get; init; }
    public double LossProbability { get; init; }
    public double SmallWinProbability { get; init; }

    //Not stored directly but calculated from the other two probabilities to ensure they always sum up to 1
    public double BigWinProbability => 1 - LossProbability - SmallWinProbability;

    public decimal SmallWinMinMultiplier { get; init; }
    public decimal SmallWinMaxMultiplier { get; init; }
    public decimal BigWinMinMultiplier { get; init; }
    public decimal BigWinMaxMultiplier { get; init; }
}