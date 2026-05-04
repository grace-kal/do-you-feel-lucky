using DoYouFeelLucky.App.Configuration;
using DoYouFeelLucky.App.Interfaces;
using DoYouFeelLucky.App.Models;
using DoYouFeelLucky.App.Services;
using DoYouFeelLucky.UnitTests.Helpers;
using DoYouFeelLucky.Wallet.Common;
using DoYouFeelLucky.Wallet.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DoYouFeelLucky.UnitTests;

public class GameServiceTests
{
    private readonly Mock<IWalletService> _mockWalletService;
    private readonly Mock<IRngService> _mockRngService;
    private readonly IGameService _gameService;
    private readonly Guid _walletId = Guid.NewGuid();
    private readonly Guid _correlationId = Guid.NewGuid();
    private const decimal DefaultBetAmount = 5;

    private readonly GameSettings _settings = TestHelper.GameSettings;

    public GameServiceTests()
    {
        _mockWalletService = new Mock<IWalletService>();
        _mockRngService = new Mock<IRngService>();

        var options = Options.Create(_settings);
        _gameService = new GameService(_mockWalletService.Object, _mockRngService.Object, options);

        _mockWalletService
            .Setup(w => w.WithdrawAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()))
            .ReturnsAsync(new WalletOperationResult { Success = true, NewBalance = 90 });

        _mockWalletService
            .Setup(w => w.DepositAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()))
            .ReturnsAsync(new WalletOperationResult { Success = true, NewBalance = 100 });
    }

    #region Bet validation tests

    [Theory]
    [InlineData(0)]
    [InlineData(0.5)]
    [InlineData(11)]
    [InlineData(-1)]
    public async Task PlaceBet_InvalidAmount_ShouldFail(decimal amount)
    {
        var result = await _gameService.PlaceBetAsync(_walletId, _correlationId, amount);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task PlaceBet_ValidAmount_ShouldSucceed(decimal amount)
    {
        _mockRngService.Setup(r => r.Roll()).Returns(0.1);
        _mockRngService.Setup(r => r.GetRandomDecimal(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(0);

        var result = await _gameService.PlaceBetAsync(_walletId, _correlationId, amount);

        Assert.True(result.Success);
    }

    #endregion

    #region Outcome tests

    [Fact]
    public async Task PlaceBet_WhenRollBelow50Percent_ShouldLose()
    {
        _mockRngService.Setup(r => r.Roll()).Returns(0.49);

        var result = await _gameService.PlaceBetAsync(_walletId, _correlationId, DefaultBetAmount);

        Assert.True(result.Success);
        Assert.Equal(OutcomeType.Loss, result.Outcome);
        Assert.Equal(0, result.WinAmount);
        Assert.Equal(0, result.Multiplier);
    }

    [Fact]
    public async Task PlaceBet_WhenRollBetween50And90Percent_ShouldSmallWin()
    {
        _mockRngService.Setup(r => r.Roll()).Returns(0.6); 
        _mockRngService.Setup(r => r.GetRandomDecimal(1, 2)).Returns(1.5m);

        var result = await _gameService.PlaceBetAsync(_walletId, _correlationId, DefaultBetAmount);

        Assert.True(result.Success);
        Assert.Equal(OutcomeType.SmallWin, result.Outcome);
        Assert.Equal(1.5m, result.Multiplier);
        Assert.Equal(7.5m, result.WinAmount);
    }

    [Fact]
    public async Task PlaceBet_WhenRollAbove90Percent_ShouldBigWin()
    {
        _mockRngService.Setup(r => r.Roll()).Returns(0.95);
        _mockRngService.Setup(r => r.GetRandomDecimal(2, 10)).Returns(5m);

        var result = await _gameService.PlaceBetAsync(_walletId, _correlationId, DefaultBetAmount);

        Assert.True(result.Success);
        Assert.Equal(OutcomeType.BigWin, result.Outcome);
        Assert.Equal(5m, result.Multiplier);
        Assert.Equal(25m, result.WinAmount);
    }

    #endregion

    #region Wallet interaction tests

    [Fact]
    public async Task PlaceBet_WhenWalletDebitFails_ShouldFail()
    {
        _mockWalletService
            .Setup(w => w.WithdrawAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()))
            .ReturnsAsync(new WalletOperationResult { Success = false, ErrorMessage = "Insufficient funds." });

        var result = await _gameService.PlaceBetAsync(_walletId, _correlationId, 5);

        Assert.False(result.Success);
        Assert.Equal("Insufficient funds.", result.ErrorMessage);
    }

    [Fact]
    public async Task PlaceBet_WhenLoss_ShouldNotCreditWallet()
    {
        _mockRngService.Setup(r => r.Roll()).Returns(0.1);

        await _gameService.PlaceBetAsync(_walletId, _correlationId, 5);

        _mockWalletService.Verify(
            w => w.DepositAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()),
            Times.Never);
    }

    #endregion
}