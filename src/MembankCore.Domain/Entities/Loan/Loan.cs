using MembankCore.Domain.Entities.Economic;

namespace MembankCore.Domain.Entities.Loan {
  public enum LoanStatus {
    Pending,
    Approved,
    Rejected,
    Disbursed,
    Closed
  }

  public class Loan(int borrowerId, decimal amount, Currency currency, decimal interestRate, LoanStatus status = LoanStatus.Pending) {
    public Guid Id { get; set; }
    public int BorrowerId { get; set; } = borrowerId; // Кто взял кредит

    public decimal PrincipalAmount { get; set; } = amount; // Тело кредита
    public decimal RemainingAmount { get; set; } = amount;// Сколько осталось вернуть (с процентами)
    public Currency? Currency { get; set; } = currency;
    public string CurrencyId { get; set; } = currency.Id;

    public decimal InterestRate { get; set; } = interestRate; // Ставка (например, 0.15 для 15%)
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastInterestAccrual { get; set; } = DateTime.UtcNow; // Когда последний раз капал процент
    public LoanStatus Status { get; set; } = status;

    // Методы

    public void Increment(DateTime now, int daysInYear) {
      if (Status != LoanStatus.Approved)
        throw new InvalidOperationException("Кредит не открыт");
      decimal dailyDebt = (PrincipalAmount * InterestRate) / daysInYear;
      RemainingAmount += dailyDebt;
      LastInterestAccrual = now;
    }

    public void Repay(decimal amount) {
      if (Status != LoanStatus.Approved)
        throw new InvalidOperationException("Кредит не открыт");
      if (amount < 0)
        throw new ArgumentException($"Сумма погашения не может быть отрицательной.");
      if (amount > RemainingAmount)
        throw new InvalidOperationException($"Кредит слишком маленький для этой суммы. Чтобы его закрыть требуется {RemainingAmount}, а не {amount}.");
      RemainingAmount -= amount;
      if (RemainingAmount == 0) {
        Status = LoanStatus.Closed;
      }
    }
  }
}
