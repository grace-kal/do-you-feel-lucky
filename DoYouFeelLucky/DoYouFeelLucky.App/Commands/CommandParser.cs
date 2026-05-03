using DoYouFeelLucky.App.Configuration;
using DoYouFeelLucky.App.Interfaces;
using DoYouFeelLucky.Wallet.Interfaces;
using DoYouFeelLucky.App.Constants;
using Microsoft.Extensions.Options;


namespace DoYouFeelLucky.App.Commands;

public class CommandParser(IWalletService walletService, IGameService gameService, IOptions<GameSettings> optionsSettings, Guid walletId)
{
    private readonly GameSettings settings = optionsSettings.Value;

    public ICommand Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new InvalidCommand(Messages.Commands.EmptyInput);

        var parts = input.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0];

        return command switch
        {
            "exit" => new ExitCommand(),
            "deposit" => ParseAmountCommand(parts, CreateDepositCommand),
            "withdraw" => ParseAmountCommand(parts, CreateWithdrawCommand),
            "bet" => ParseBetCommand(parts),
            _ => new InvalidCommand(string.Format(Messages.Commands.UnknownCommand, command))
        };
    }

    private ICommand ParseAmountCommand(string[] parts, Func<decimal, ICommand> createCommand)
    {
        if (parts.Length < 2)
            return new InvalidCommand(Messages.Commands.ProvideValidAmount);

        if (parts.Length > 2)
            return new InvalidCommand(Messages.Commands.InvalidFormat);

        if (!decimal.TryParse(parts[1], out var amount))
            return new InvalidCommand(Messages.Commands.AmountMustBeNumber);

        if (amount <= 0)
            return new InvalidCommand(Messages.Commands.AmountMustBePositive);

        return createCommand(amount);
    }

    private ICommand ParseBetCommand(string[] parts)
    {
        if (parts.Length < 2)
            return new InvalidCommand(Messages.Commands.ProvideValidAmount);

        if (parts.Length > 2)
            return new InvalidCommand(Messages.Commands.InvalidFormat);

        if (!decimal.TryParse(parts[1], out var amount))
            return new InvalidCommand(Messages.Commands.AmountMustBeNumber);

        if (amount <= 0)
            return new InvalidCommand(Messages.Commands.AmountMustBePositive);

        if (amount < settings.MinBet || amount > settings.MaxBet)
            return new InvalidCommand(string.Format(Messages.Game.BetOutOfRange, settings.MinBet, settings.MaxBet));

        return new BetCommand(gameService, walletId, amount);
    }

    private ICommand CreateDepositCommand(decimal amount) =>
       new DepositCommand(walletService, walletId, amount);

    private ICommand CreateWithdrawCommand(decimal amount) =>
        new WithdrawCommand(walletService, walletId, amount);

}
