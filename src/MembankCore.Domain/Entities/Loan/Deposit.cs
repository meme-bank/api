using MembankCore.Domain.Entities.Economic;

namespace MembankCore.Domain.Entities.Loan {
  public class Deposit(int ownerId, decimal amount, Currency currency, decimal interestRate = 0.05m) {
    public Guid Id { get; set; }
    public int OwnerId { get; set; } = ownerId;
    public decimal Amount { get; set; } = amount;
    public Currency? Currency { get; set; } = currency;
    public string CurrencyId { get; set; } = currency.Id;
    public decimal InterestRate { get; set; } = interestRate;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastInterestAccrual { get; set; } = DateTime.UtcNow;
    public DateTime MaturityDate { get; set; } = DateTime.UtcNow.AddDays(7);
    public bool IsRenewed { get; set; } = true;

    public void Increment(DateTime now, int daysInYear) {
      decimal dailyInterest = (Amount * InterestRate) / daysInYear;
      Amount += dailyInterest;
      LastInterestAccrual = now;
    }
  }
}
