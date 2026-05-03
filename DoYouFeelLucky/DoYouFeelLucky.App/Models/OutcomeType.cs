using System.ComponentModel;

namespace DoYouFeelLucky.App.Models;

public enum OutcomeType
{
    [Description("No luck this time!")]
    Loss,
    [Description("Congrats - you won")]
    SmallWin,
    [Description("Congrats - you won")]
    BigWin
}