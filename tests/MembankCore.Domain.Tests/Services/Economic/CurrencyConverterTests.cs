using MembankCore.Domain.Entities.Core;
using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.Services.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Tests.Services.Economic;

public class CurrencyConverterTests
{
    private readonly CurrencyConverter _converter;

    private Currency CreateCurrency(string code, decimal rate) {
      return new Currency(
        code,
        code,
        rate,
        new Photo(1, PhotoType.Icon)
      );
    }

    public CurrencyConverterTests()
    {
        _converter = new CurrencyConverter();
    }

    [Fact]
    public void Convert_ShouldReturnSameAmount_WhenCurrenciesAreIdentical()
    {
        // Arrange
        var usd = CreateCurrency ("USD", 1.0m);
        var sourceMoney = new Money(100m, "USD");

        // Act
        var result = _converter.Convert(sourceMoney, usd, usd);

        // Assert
        Assert.Equal(sourceMoney.Amount, result.Amount);
        Assert.Equal(sourceMoney.CurrencyId, result.CurrencyId);
    }

    [Fact]
    public void Convert_ShouldCalculateCorrectly_WhenConvertingViaBaseRate()
    {
        // Переводим 100 EUR в USD
        // Курс EUR: 1.1 (1 EUR = 1.1 USD)
        // Курс USD: 1.0 (Базовая валюта)
        // Формула: (100 * 1.1) / 1.0 = 110

        // Arrange
        var eur = CreateCurrency("EUR", 1.1m);
        var usd = CreateCurrency("USD", 1.0m);
        var sourceMoney = new Money(100m, "EUR");

        // Act
        var result = _converter.Convert(sourceMoney, eur, usd);

        // Assert
        Assert.Equal(110m, result.Amount);
        Assert.Equal("USD", result.CurrencyId);
    }

    [Fact]
    public void Convert_ShouldHandleCrossRate_Correctly()
    {
        // Переводим 100 EUR в GBP через базовую валюту (например, USD)
        // EUR курс 1.2, GBP курс 0.8
        // Формула: (100 * 1.2) / 0.8 = 150

        // Arrange
        var eur = CreateCurrency("EUR", 1.2m);
        var gbp = CreateCurrency("GBP", 0.8m);
        var sourceMoney = new Money(100m, "EUR");

        // Act
        var result = _converter.Convert(sourceMoney, eur, gbp);

        // Assert
        Assert.Equal(150m, result.Amount);
        Assert.Equal("GBP", result.CurrencyId);
    }

    [Fact]
    public void Convert_ShouldThrowArgumentException_WhenMoneyCurrencyDoesNotMatchSourceCurrency()
    {
        // Arrange
        var usd = CreateCurrency("USD", 1.0m);
        var eur = CreateCurrency("EUR", 0.9m);
        var moneyInGbp = new Money(100m, "GBP"); // Валюта не совпадает с usd

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _converter.Convert(moneyInGbp, usd, eur));

        Assert.Contains("Исходная валюта Money не совпадает", exception.Message);
    }

    [Fact]
    public void Convert_ShouldThrowDivideByZeroException_WhenTargetRateIsZero()
    {
        // Arrange
        var usd = CreateCurrency("USD", 1.0m);
        var brokenCurrency = CreateCurrency("ERR", 0m); // Курс ноль
        var money = new Money(100m, "USD");

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() =>
            _converter.Convert(money, usd, brokenCurrency));
    }
}
