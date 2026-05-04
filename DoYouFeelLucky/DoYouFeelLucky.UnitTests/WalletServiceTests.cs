using DoYouFeelLucky.Wallet.Services;
using Xunit;

namespace DoYouFeelLucky.UnitTests;

public class WalletServiceTests
{
    private readonly WalletService _walletService;
    private readonly Guid _walletId = Guid.NewGuid();
    private readonly Guid _correlationId = Guid.NewGuid();
    private readonly Guid _idempotencyKey = Guid.NewGuid();

    public WalletServiceTests() => _walletService = new WalletService();

    #region Deposit tests

    [Theory]
    [InlineData(10, true, 10)]
    [InlineData(50, true, 50)]
    [InlineData(0.01, true, 0.01)]
    public async Task Deposit_ValidAmount_ShouldSucceedAndUpdateBalance(decimal amount, bool expectedSuccess, decimal expectedBalance)
    {
        var result = await _walletService.DepositAsync(_walletId, _correlationId, _idempotencyKey, amount);

        Assert.Equal(expectedSuccess, result.Success);
        Assert.Equal(expectedBalance, result.NewBalance);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Deposit_InvalidAmount_ShouldFail(decimal amount)
    {
        var result = await _walletService.DepositAsync(_walletId, _correlationId, _idempotencyKey, amount);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    #endregion

    #region Withdraw tests

    [Theory]
    [InlineData(100, 30, true, 70)]
    [InlineData(100, 100, true, 0)]
    [InlineData(100, 0.01, true, 99.99)]
    public async Task Withdraw_ValidAmount_ShouldSucceedAndUpdateBalance(decimal depositAmount, decimal withdrawAmount, bool expectedSuccess, decimal expectedBalance)
    {
        await _walletService.DepositAsync(_walletId, _correlationId, _idempotencyKey, depositAmount);

        var result = await _walletService.WithdrawAsync(_walletId, _correlationId, _idempotencyKey, withdrawAmount);

        Assert.Equal(expectedSuccess, result.Success);
        Assert.Equal(expectedBalance, result.NewBalance);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Withdraw_InvalidAmount_ShouldFail(decimal amount)
    {
        var result = await _walletService.WithdrawAsync(_walletId, _correlationId, _idempotencyKey, amount);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Theory]
    [InlineData(10, 20)]
    [InlineData(10, 10.01)]
    [InlineData(0, 1)]
    public async Task Withdraw_InsufficientFunds_ShouldFail(decimal depositAmount, decimal withdrawAmount)
    {
        await _walletService.DepositAsync(_walletId, _correlationId, _idempotencyKey, depositAmount);

        var result = await _walletService.WithdrawAsync(_walletId, _correlationId, _idempotencyKey, withdrawAmount);

        Assert.False(result.Success);
        Assert.Equal("Insufficient funds.", result.ErrorMessage);
    }

    #endregion

    #region Balance tests

    [Fact]
    public async Task Balance_StartsAtZero()
    {
        var result = await _walletService.DepositAsync(_walletId, _correlationId, _idempotencyKey, 0.01m);
        var balanceBeforeDeposit = result.NewBalance - 0.01m;

        Assert.Equal(0, balanceBeforeDeposit);
    }

    [Fact]
    public async Task Balance_AfterMultipleOperations_ShouldBeCorrect()
    {
        await _walletService.DepositAsync(_walletId, _correlationId, Guid.NewGuid(), 100);
        await _walletService.WithdrawAsync(_walletId, _correlationId, Guid.NewGuid(), 30);
        await _walletService.DepositAsync(_walletId, _correlationId, Guid.NewGuid(), 50);
        var result = await _walletService.WithdrawAsync(_walletId, _correlationId, Guid.NewGuid(), 20);

        Assert.True(result.Success);
        Assert.Equal(100, result.NewBalance);
    }

    #endregion
}
