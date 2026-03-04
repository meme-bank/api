using MembankCore.Domain.Entities.Core;
using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;
using Xunit;

namespace MembankCore.Domain.Tests.Entities.Economic;

public class WalletTests {
  private readonly Currency _testCurrency = new("LMC", "Левро", 1.0m, 1, new(1, PhotoType.Icon));

  private Wallet CreateTestWallet(decimal initialAmount = 0) {
    Money initialBalance = new(initialAmount, _testCurrency.Id);
    return new Wallet(1, _testCurrency, initialBalance, "Test");
  }

  [Fact]
  public void Deposit_ShouldIncreaseBalance_WhenAmountIsPositive() {
    // Arrange
    var wallet = CreateTestWallet(100m);
    var depositAmount = new Money(50m, _testCurrency.Id);

    // Act
    wallet.Deposit(depositAmount);

    // Assert
    Assert.Equal(150m, wallet.Balance.Amount);
    Assert.Equal(_testCurrency.Id, wallet.Balance.CurrencyId);
  }

  [Fact]
  public void Withdraw_ShouldAllowEmptyingBalance_WhenAmountIsExactlyBalance() {
    // Arrange
    var wallet = CreateTestWallet(100m);
    var fullAmount = new Money(100m, _testCurrency.Id);

    // Act
    wallet.Withdraw(fullAmount);

    // Assert
    Assert.Equal(0m, wallet.Balance.Amount);
  }

  [Fact]
  public void Withdraw_WithWrongCurrency_ShouldThrowArgumentException() {
    // Arrange
    var wallet = CreateTestWallet(100m);
    var foreignMoney = new Money(10m, "USD"); // Пытаемся списать USD с кошелька LMC

    // Act & Assert
    var ex = Assert.Throws<ArgumentException>(() => wallet.Withdraw(foreignMoney));
    Assert.Contains("валют", ex.Message.ToLower());
  }

  [Theory]
  [InlineData(0)]
  public void Deposit_ShouldThrowArgumentException_WhenAmountIsZero(decimal zeroValue) {
    // Arrange
    var wallet = CreateTestWallet();
    var zeroMoney = new Money(zeroValue, _testCurrency.Id);

    // Act & Assert
    Assert.Throws<ArgumentException>(() => wallet.Deposit(zeroMoney));
  }

  [Fact]
  public void Withdraw_ShouldDecreaseBalance_WhenFundsAreSufficient() {
    // Arrange
    var wallet = CreateTestWallet(100m);
    var withdrawAmount = new Money(30m, _testCurrency.Id);

    // Act
    wallet.Withdraw(withdrawAmount);

    // Assert
    Assert.Equal(70m, wallet.Balance.Amount);
  }

  [Fact]
  public void Withdraw_ShouldThrowInvalidOperationException_WhenFundsAreInsufficient() {
    // Arrange
    var wallet = CreateTestWallet(50m);
    var withdrawAmount = new Money(100m, _testCurrency.Id);

    // Act & Assert
    var exception = Assert.Throws<InvalidOperationException>(() =>
                        wallet.Withdraw(withdrawAmount));

    Assert.Contains("Недостаточно средств", exception.Message);
  }

  [Theory]
  [InlineData(-10)]
  [InlineData(-0.01)]
  public void Deposit_ShouldThrowArgumentException_WhenAmountIsNegative(decimal negativeValue) {
    // Arrange
    var wallet = CreateTestWallet();
    var negativeMoney = new Money(negativeValue, _testCurrency.Id);

    // Act & Assert
    Assert.Throws<ArgumentException>(() => wallet.Deposit(negativeMoney));
  }

  [Fact]
  public void Operation_WithWrongCurrency_ShouldThrow() {
    // Arrange
    var wallet = CreateTestWallet(100m);
    var usdMoney = new Money(50m, "USD");

    // Act & Assert
    Assert.Throws<ArgumentException>(() => wallet.Deposit(usdMoney));
  }
}
