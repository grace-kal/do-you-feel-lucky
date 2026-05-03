using DoYouFeelLucky.App.Commands;
using DoYouFeelLucky.App.Constants;

namespace DoYouFeelLucky.App;

public class ConsoleGameRunner(CommandParser parser)
{
    public async Task RunAsync()
    {
        Console.WriteLine(Messages.Runner.Welcome);
        Console.WriteLine(Messages.Runner.Commands);
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine(Messages.Runner.SubmitAction);
            var input = Console.ReadLine();

            var command = parser.Parse(input ?? string.Empty);
            var result = await command.ExecuteAsync();

            Console.WriteLine(result);
            Console.WriteLine();

            if (command is ExitCommand)
                break;
        }

        Console.WriteLine(Messages.Runner.PressAnyKey);
        Console.ReadKey();
    }
}