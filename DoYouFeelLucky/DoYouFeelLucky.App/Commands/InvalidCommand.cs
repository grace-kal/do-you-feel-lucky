namespace DoYouFeelLucky.App.Commands;

public class InvalidCommand(string errorMessage) : ICommand
{
    public Task<string> ExecuteAsync() => Task.FromResult(errorMessage);
}
