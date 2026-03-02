using MembankCore.Domain.Entities.Economic;
using Xunit;

namespace MembankCore.Domain.Tests.Entities.Economic;

public class WalletTests {
  private Wallet CreateTestWallet(decimal initialBalance = 0) {
    return new Wallet(1, new("LMC", "Левро"), initialBalance, "Test");
  }

  [Fact]
  public void Deposit_ShouldIncreaseBalance_WhenAmountIsPositive() {
    // Arrange
    var wallet = CreateTestWallet(100m);
    decimal depositAmount = 50m;

    // Act
    wallet.Deposit(depositAmount);

    // Assert
    Assert.Equal(150m, wallet.Balance);
  }

  [Fact]
  public void Withdraw_ShouldDecreaseBalance_WhenFundsAreSufficient() {
    // Arrange
    var wallet = CreateTestWallet(100m);
    decimal withdrawAmount = 30m;

    // Act
    wallet.Withdraw(withdrawAmount);

    // Assert
    Assert.Equal(70m, wallet.Balance);
  }

  [Fact]
  public void Withdraw_ShouldThrowInvalidOperationException_WhenFundsAreInsufficient() {
    // Arrange
    var wallet = CreateTestWallet(50m);
    decimal withdrawAmount = 100m;

    // Act & Assert
    var exception = Assert.Throws<InvalidOperationException>(() =>
                    wallet.Withdraw(withdrawAmount));

    Assert.Contains("Недостаточно средств", exception.Message);
  }

  [Theory]
  [InlineData(-10)]
  [InlineData(-0.01)]
  public void Deposit_ShouldThrowArgumentException_WhenAmountIsNegative(decimal negativeAmount) {
    // Arrange
    var wallet = CreateTestWallet();

    // Act & Assert
    Assert.Throws<ArgumentException>(() => wallet.Deposit(negativeAmount));
  }
}
