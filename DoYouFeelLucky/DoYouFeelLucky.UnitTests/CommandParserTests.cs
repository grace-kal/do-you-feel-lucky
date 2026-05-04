using DoYouFeelLucky.App.Commands;
using DoYouFeelLucky.App.Configuration;
using DoYouFeelLucky.App.Interfaces;
using DoYouFeelLucky.UnitTests.Helpers;
using DoYouFeelLucky.Wallet.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DoYouFeelLucky.UnitTests;

public class CommandParserTests
{
    private readonly CommandParser _parser;
    private readonly Guid _walletId = Guid.NewGuid();

    private readonly GameSettings _settings = TestHelper.GameSettings;
    public CommandParserTests()
    {
        var mockWalletService = new Mock<IWalletService>();
        var mockGameService = new Mock<IGameService>();
        var options = Options.Create(_settings);

        _parser = new CommandParser(
            mockWalletService.Object,
            mockGameService.Object,
            options,
            _walletId);
    }

    #region Valid command tests

    [Theory]
    [InlineData("deposit 10", typeof(DepositCommand))]
    [InlineData("withdraw 10", typeof(WithdrawCommand))]
    [InlineData("bet 5", typeof(BetCommand))]
    [InlineData("exit", typeof(ExitCommand))]
    public void Parse_ValidCommand_ShouldReturnCorrectCommandType(string input, Type expectedType)
    {
        var command = _parser.Parse(input);

        Assert.IsType(expectedType, command);
    }

    [Theory]
    [InlineData("DEPOSIT 10")]
    [InlineData("Deposit 10")]
    [InlineData("EXIT")]
    [InlineData("Exit")]
    public void Parse_CaseInsensitiveInput_ShouldReturnValidCommand(string input)
    {
        var command = _parser.Parse(input);

        Assert.IsNotType<InvalidCommand>(command);
    }

    #endregion

    #region Invalid command tests

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_EmptyOrNullInput_ShouldReturnInvalidCommand(string input)
    {
        var command = _parser.Parse(input);

        Assert.IsType<InvalidCommand>(command);
    }

    [Theory]
    [InlineData("hello")]
    [InlineData("play")]
    [InlineData("start")]
    public void Parse_UnknownCommand_ShouldReturnInvalidCommand(string input)
    {
        var command = _parser.Parse(input);

        Assert.IsType<InvalidCommand>(command);
    }

    #endregion

    #region Amount validation tests

    [Theory]
    [InlineData("deposit")]
    [InlineData("withdraw")]
    public void Parse_MissingAmount_ShouldReturnInvalidCommand(string input)
    {
        var command = _parser.Parse(input);

        Assert.IsType<InvalidCommand>(command);
    }

    [Theory]
    [InlineData("deposit abc")]
    [InlineData("withdraw abc")]
    [InlineData("bet abc")]
    public void Parse_NonNumericAmount_ShouldReturnInvalidCommand(string input)
    {
        var command = _parser.Parse(input);

        Assert.IsType<InvalidCommand>(command);
    }

    [Theory]
    [InlineData("deposit -10")]
    [InlineData("withdraw -10")]
    [InlineData("bet -5")]
    [InlineData("deposit 0")]
    [InlineData("withdraw 0")]
    public void Parse_NegativeOrZeroAmount_ShouldReturnInvalidCommand(string input)
    {
        var command = _parser.Parse(input);

        Assert.IsType<InvalidCommand>(command);
    }

    [Theory]
    [InlineData("bet 0.5")]
    [InlineData("bet 11")]
    [InlineData("bet 100")]
    public void Parse_BetOutOfRange_ShouldReturnInvalidCommand(string input)
    {
        var command = _parser.Parse(input);

        Assert.IsType<InvalidCommand>(command);
    }

    [Theory]
    [InlineData("deposit 10 20")]
    [InlineData("withdraw 10 20")]
    [InlineData("bet 5 10")]
    public void Parse_TooManyArguments_ShouldReturnInvalidCommand(string input)
    {
        var command = _parser.Parse(input);

        Assert.IsType<InvalidCommand>(command);
    }

    #endregion
}
