using MembankCore.Domain.Entities.Core;
using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.Interfaces.Services.Economic;
using MembankCore.Domain.ValueObjects;
using Moq;
using TransferProcessor = MembankCore.Domain.Services.Economic.TransferProcessor;

namespace MembankCore.Domain.Tests.Services.Economic;

public class TransferProcessorTests {
  private readonly Mock<ICurrencyConverter> _converterMock;
  private readonly TransferProcessor _processor;

  private Currency CreateCurrency(string code, decimal rate) {
    return new Currency(
      code,
      code,
      rate,
      new Photo(1, PhotoType.Icon)
    );
  }

  private Wallet CreateWallet(Currency currency, decimal balance) {
    return new Wallet(
      1,
      currency,
      new Money(balance, currency.Id),
      "Test"
    );
  }

  public TransferProcessorTests() {
    _converterMock = new Mock<ICurrencyConverter>();
    _processor = new TransferProcessor(_converterMock.Object);
  }

  [Fact]
  public void Execute_ShouldApplyFivePercentFee_WhenNoPrimeAndPersonalTransfer() {
    // Arrange
    var usd = CreateCurrency("USD", 1.0m);

    var sender = CreateWallet(usd, 1000m);
    var receiver = CreateWallet(usd, 0m);
    var amount = new Money(100m, "USD");

    SetupConvertPassThroughAsync();

    // Act
    var result = _processor.Execute(sender, receiver, amount, hasPrime: false, TransferNoteType.Personal);

    // Assert
    Assert.Equal(5m, result.Fee.Amount); // 5% от 100
    Assert.Equal(105m, result.SentAmount.Amount);
    Assert.Equal(895m, sender.Balance.Amount); // 1000 - 105
    Assert.Equal(100m, receiver.Balance.Amount);
  }

  [Theory]
  [InlineData(true, TransferNoteType.Personal)] // Есть Prime
  [InlineData(false, TransferNoteType.Buying)] // Не личный перевод
  [InlineData(true, TransferNoteType.Buying)] // И то, и другое
  public void Execute_ShouldNotApplyFee_WhenConditionsMet(bool hasPrime, TransferNoteType type) {
    // Arrange
    var usd = CreateCurrency("USD", 1.0m);

    var sender = CreateWallet(usd, 500m);
    var receiver = CreateWallet(usd, 0m);
    var amount = new Money(100m, "USD");

    SetupConvertPassThroughAsync();

    // Act
    var result = _processor.Execute(sender, receiver, amount, hasPrime, type);

    // Assert
    Assert.Equal(0m, result.Fee.Amount);
    Assert.Equal(100m, result.SentAmount.Amount);
    Assert.Equal(400m, sender.Balance.Amount);
  }

  [Fact]
  public void Execute_ShouldHandleCurrencyConversion_WhenCurrenciesDiffer() {
    // Arrange
    var usd = CreateCurrency("USD", 1.0m);
    var eur = CreateCurrency("EUR", 0.9m);

    var sender = CreateWallet(usd, 1000m);
    var receiver = CreateWallet(eur, 0m);
    var amountInUsd = new Money(100m, "USD");

    // Настраиваем конвертацию: 100 USD -> 90 EUR
    _converterMock.Setup(c => c.Convert(It.IsAny<Money>(), usd, eur))
        .Returns(new Money(90m, "EUR"));

    // Настраиваем списание (в той же валюте)
    _converterMock.Setup(c => c.Convert(It.IsAny<Money>(), usd, usd))
        .Returns((Money m, Currency f, Currency t) => m);

    // Act
    var result = _processor.Execute(sender, receiver, amountInUsd, hasPrime: true, TransferNoteType.Personal);

    // Assert
    Assert.Equal(90m, result.ReceivedAmount.Amount);
    Assert.Equal("EUR", result.ReceivedAmount.CurrencyId);
    Assert.Equal(90m, receiver.Balance.Amount);
    Assert.Equal(0.9m, result.RateAtTransfer);
  }

  // --- Вспомогательные методы ---

  private void SetupConvertPassThroughAsync() {
    _converterMock.Setup(c => c.Convert(It.IsAny<Money>(), It.IsAny<Currency>(), It.IsAny<Currency>()))
        .Returns((Money m, Currency from, Currency to) => new Money(m.Amount, to.Id));
  }
}
