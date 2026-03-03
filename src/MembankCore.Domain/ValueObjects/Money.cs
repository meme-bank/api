using MembankCore.Domain.Services.Economic;
using MembankCore.Domain.Entities.Economic;

namespace MembankCore.Domain.ValueObjects;

public record Money : IComparable<Money>
{
  public decimal Amount { get; init; }
  public string CurrencyId { get; init; }

  public Money ConvertTo(Currency targetCurrency, Currency sourceCurrency, CurrencyConverter converter)
  {
    return converter.Convert(this, sourceCurrency, targetCurrency);
  }

  public Money(decimal amount, string currencyId)
  {
    if (string.IsNullOrWhiteSpace(currencyId))
      throw new Exception("CurrencyId обязателен для создания денежной суммы.");

    Amount = amount;
    CurrencyId = currencyId;
  }

  public int CompareTo(Money? other)
  {
    if (other is null) return 1;
    if (CurrencyId != other.CurrencyId)
      throw new InvalidOperationException($"Нельзя сравнивать разные валюты: {CurrencyId} и {other.CurrencyId}");

    return Amount.CompareTo(other.Amount);
  }

  public static Money Create(decimal amount, Currency currency)
      => new(amount, currency.Id);

  // Арифметика
  public static Money operator +(Money a, Money b) =>
    a.CurrencyId == b.CurrencyId ? a with { Amount = a.Amount + b.Amount } : throw new InvalidOperationException($"Несоответствие валют: {a.CurrencyId} и {b.CurrencyId}");
  public static Money operator *(Money m, decimal factor) => m with { Amount = m.Amount * factor };
  public static Money operator /(Money m, decimal divisor) => m with { Amount = m.Amount / divisor };
  public static Money operator -(Money a, Money b) =>
    a.CurrencyId == b.CurrencyId ? a with { Amount = a.Amount - b.Amount } : throw new InvalidOperationException($"Несоответствие валют: {a.CurrencyId} и {b.CurrencyId}");

  // Сравнение
  // меж Money

  public static bool operator <(Money a, Money b) => a.CompareTo(b) < 0;
  public static bool operator >(Money a, Money b) => a.CompareTo(b) > 0;
  public static bool operator <=(Money a, Money b) => a.CompareTo(b) <= 0;
  public static bool operator >=(Money a, Money b) => a.CompareTo(b) >= 0;

  // меж числом и Money

  public static bool operator <(Money a, decimal b) => a.Amount < b;
  public static bool operator >(Money a, decimal b) => a.Amount > b;
  public static bool operator ==(Money a, decimal b) => a.Amount == b;
  public static bool operator !=(Money a, decimal b) => a.Amount != b;
}
