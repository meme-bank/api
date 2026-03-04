using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Loan;

/// <summary>
/// Класс вклада
/// </summary>
public class Deposit {
  public Guid Id { get; private set; }
  public int OwnerId { get; private set; }
  public Money Amount { get; private set; }

  public string CurrencyId => Amount.CurrencyId;
  public virtual Currency Currency { get; private set; }

  public decimal InterestRate { get; private set; }
  public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
  public DateTime LastInterestAccrual { get; private set; } = DateTime.UtcNow;
  public DateTime LastAmountAccrual { get; private set; }
  public DateTime MaturityDate { get; private set; } = DateTime.UtcNow.AddDays(7);
  public bool IsRenewed { get; private set; } = true;

  public Deposit(int ownerId, Money amount, Currency currency, decimal interestRate = 0.05m) {
    if (amount.CurrencyId != currency.Id)
      throw new Exception("Валюта суммы не совпадает с объектом валюты.");

    OwnerId = ownerId;
    Amount = amount;
    Currency = currency;
    InterestRate = interestRate;
  }

  /// <summary>
  /// Начисление процента по вкладу (процент хранится в <see cref="InterestRate"/>
  /// </summary>
  /// <param name="now">Во сколько было начислено</param>
  /// <param name="daysInYear">Сколько дней в году (по РП)</param>
  public void Increment(DateTime now, int daysInYear) {
    Money dailyInterest = (Amount * InterestRate) / daysInYear;
    Amount += dailyInterest;
    LastInterestAccrual = now;
  }

  public void ChangeInterestRate(decimal interestRate) {
    if (interestRate <= 0)
      throw new ArgumentException("Процент вклада должен быть положительным");
    InterestRate = interestRate;
  }

  public void AddAmount(Money amount) {
    if (amount <= 0)
      throw new ArgumentException("Сумма ввода должна быть положительной.");
    Amount += amount;
    LastAmountAccrual = DateTime.UtcNow;
  }

  public void Withdraw(Money amount) {
    if (amount <= 0)
      throw new ArgumentException("Сумма вывода должна быть положительной.");
    Amount += amount;
    LastAmountAccrual = DateTime.UtcNow;
  }
}
