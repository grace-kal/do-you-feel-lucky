namespace DoYouFeelLucky.App.Commands;

public interface ICommand
{
    Task<string> ExecuteAsync();
}