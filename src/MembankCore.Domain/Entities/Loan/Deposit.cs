using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Loan
{
  public class Deposit
  {
    public Guid Id { get; private set; }
    public int OwnerId { get; private set; }
    public Money Amount { get; private set; }

    public string CurrencyId => Amount.CurrencyId;
    public virtual Currency Currency { get; private set; }

    public decimal InterestRate { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime LastInterestAccrual { get; private set; } = DateTime.UtcNow;
    public DateTime MaturityDate { get; private set; } = DateTime.UtcNow.AddDays(7);
    public bool IsRenewed { get; private set; } = true;

    public Deposit(int ownerId, Money amount, Currency currency, decimal interestRate = 0.05m)
    {
      if (amount.CurrencyId != currency.Id)
        throw new Exception("Валюта суммы не совпадает с объектом валюты.");

      OwnerId = ownerId;
      Amount = amount;
      Currency = currency;
      InterestRate = interestRate;
    }

    public void Increment(DateTime now, int daysInYear)
    {
      Money dailyInterest = (Amount * InterestRate) / daysInYear;
      Amount += dailyInterest;
      LastInterestAccrual = now;
    }
  }
}
