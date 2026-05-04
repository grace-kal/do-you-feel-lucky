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
            return new InvalidCommand(Messages.CommandMessages.EmptyInput);

        var parts = input.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0];

        return command switch
        {
            Messages.Commands.Exit => new ExitCommand(),
            Messages.Commands.Deposit => ParseAmountCommand(parts, CreateDepositCommand),
            Messages.Commands.Withdraw => ParseAmountCommand(parts, CreateWithdrawCommand),
            Messages.Commands.Bet => ParseBetCommand(parts),
            _ => new InvalidCommand(string.Format(Messages.CommandMessages.UnknownCommand, command))
        };
    }

    private ICommand ParseAmountCommand(string[] parts, Func<decimal, ICommand> createCommand)
    {
        if (parts.Length < 2)
            return new InvalidCommand(Messages.CommandMessages.ProvideValidAmount);

        if (parts.Length > 2)
            return new InvalidCommand(Messages.CommandMessages.InvalidFormat);

        if (!decimal.TryParse(parts[1], out var amount))
            return new InvalidCommand(Messages.CommandMessages.AmountMustBeNumber);

        if (amount <= 0)
            return new InvalidCommand(Messages.CommandMessages.AmountMustBePositive);

        return createCommand(amount);
    }

    private ICommand ParseBetCommand(string[] parts)
    {
        if (parts.Length < 2)
            return new InvalidCommand(Messages.CommandMessages.ProvideValidAmount);

        if (parts.Length > 2)
            return new InvalidCommand(Messages.CommandMessages.InvalidFormat);

        if (!decimal.TryParse(parts[1], out var amount))
            return new InvalidCommand(Messages.CommandMessages.AmountMustBeNumber);

        if (amount <= 0)
            return new InvalidCommand(Messages.CommandMessages.AmountMustBePositive);

        if (amount < settings.MinBet || amount > settings.MaxBet)
            return new InvalidCommand(string.Format(Messages.Game.BetOutOfRange, settings.MinBet, settings.MaxBet));

        return new BetCommand(gameService, walletId, amount);
    }

    private ICommand CreateDepositCommand(decimal amount) =>
       new DepositCommand(walletService, walletId, amount);

    private ICommand CreateWithdrawCommand(decimal amount) =>
        new WithdrawCommand(walletService, walletId, amount);

}
