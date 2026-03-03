using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.Interfaces.Services.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Services.Economic;

public class CurrencyConverter : ICurrencyConverter {
  public Money Convert(Money source, Currency sourceCurrency, Currency targetCurrency) {
    if (source.CurrencyId == targetCurrency.Id) return source;

    if (source.CurrencyId != sourceCurrency.Id)
      throw new ArgumentException("Исходная валюта Money не совпадает с объектом sourceCurrency.");

    decimal amountInBase = source.Amount * sourceCurrency.ExchangeRate;

    if (targetCurrency.ExchangeRate <= 0)
      throw new DivideByZeroException($"Курс целевой валюты {targetCurrency.Id} некорректен.");

    decimal convertedAmount = amountInBase / targetCurrency.ExchangeRate;

    return new Money(convertedAmount, targetCurrency.Id);
  }
}
