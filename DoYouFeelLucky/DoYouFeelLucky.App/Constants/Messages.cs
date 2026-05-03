
namespace DoYouFeelLucky.App.Constants;

public static class Messages
{
    public static class Commands
    {
        public const string EmptyInput = "Please enter a command.";
        public const string InvalidFormat = "Invalid format.";
        public const string ProvideValidAmount = "Please provide a valid amount.";
        public const string AmountMustBeNumber = "Amount must be a valid number.";
        public const string AmountMustBePositive = "Amount must be a positive number.";
        public const string UnknownCommand = "Unknown command '{0}'. Valid commands are: deposit, withdraw, bet, exit.";
    }

    public static class Wallet
    {
        public const string DepositSuccess = "Your deposit of ${0:F2} was successful. Your current balance is: ${1:F2}";
        public const string WithdrawSuccess = "Your withdrawal of ${0:F2} was successful. Your current balance is: ${1:F2}";
        public const string DepositFailed = "Deposit failed: {0}";
        public const string WithdrawFailed = "Withdrawal failed: {0}";
    }

    public static class Game
    {
        public const string BetFailed = "Bet failed: {0}";
        public const string BetOutOfRange = "Bet must be between ${0} and ${1}.";
        public const string WinMessage = "{0} ${1:F2}! Your current balance is: ${2:F2}";
        public const string LossMessage = "{0} Your current balance is: ${1:F2}";
        public const string ExitMessage = "Thank you for playing! Hope to see you again soon.";
    }

    public static class Runner
    {
        public const string Welcome = "Welcome to DoYouFeelLucky!";
        public const string Commands = "Commands: deposit <amount>, withdraw <amount>, bet <amount>, exit";
        public const string SubmitAction = "Please, submit action:";
        public const string PressAnyKey = "Press any key to exit.";
    }
}