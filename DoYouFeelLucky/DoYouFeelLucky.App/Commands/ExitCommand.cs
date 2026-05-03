using DoYouFeelLucky.App.Constants;

namespace DoYouFeelLucky.App.Commands;

public class ExitCommand : ICommand
{
    public Task<string> ExecuteAsync() => Task.FromResult(Messages.Game.ExitMessage);
}