using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Loan;

public enum LoanStatus {
  Pending,
  Approved,
  Rejected,
  Disbursed,
  Closed
}

public class Loan {
  public Guid Id { get; private set; }
  public int BorrowerId { get; private set; } // Кто взял кредит

  public Money PrincipalAmount { get; private set; } // Тело кредита
  public Money RemainingAmount { get; private set; }// Сколько осталось вернуть (с процентами)

  public string CurrencyId => RemainingAmount.CurrencyId;
  public virtual Currency Currency { get; private set; }

  public decimal InterestRate { get; private set; } // Ставка (например, 0.15 для 15%)
  public DateTime IssuedAt { get; private set; } = DateTime.UtcNow;
  public DateTime LastInterestAccrual { get; private set; } = DateTime.UtcNow; // Когда последний раз капал процент
  public LoanStatus Status { get; private set; }

  // Методы
  //
  public Loan(int borrowerId, Money amount, Currency currency, decimal interestRate, LoanStatus status = LoanStatus.Pending) {
    if (amount.CurrencyId != currency.Id)
      throw new Exception("Валюта суммы не совпадает с объектом валюты.");

    BorrowerId = borrowerId;
    InterestRate = interestRate;
    Status = status;
    PrincipalAmount = amount;
    RemainingAmount = amount;
  }

  public void Increment(DateTime now, int daysInYear) {
    if (Status != LoanStatus.Approved)
      throw new InvalidOperationException("Кредит не открыт.");
    Money dailyDebt = (PrincipalAmount * InterestRate) / daysInYear;
    RemainingAmount += dailyDebt;
    LastInterestAccrual = now;
  }

  public void Approve() {
    if (Status != LoanStatus.Pending)
      throw new InvalidOperationException("Можно одобрить только ожидающий кредит.");
    Status = LoanStatus.Approved;
  }

  public void Reject() {
    if (Status != LoanStatus.Pending)
      throw new InvalidOperationException("Нельзя отклонить уже обработанный кредит.");
    Status = LoanStatus.Rejected;
  }

  // ТОЛЬКО ДЛЯ ТЕСТОВ
  public void UpdateStatus(LoanStatus status) {
    Status = status;
  }

  public void Repay(Money amount) {
    if (Status != LoanStatus.Approved)
      throw new InvalidOperationException("Кредит не открыт.");
    if (amount.CurrencyId != this.CurrencyId)
      throw new ArgumentException($"Неверная валюта платежа. Ожидается {this.CurrencyId}, получено {amount.CurrencyId}");
    if (amount < 0)
      throw new ArgumentException($"Сумма погашения не может быть отрицательной.");
    if (amount > RemainingAmount)
      throw new ArgumentException($"Кредит слишком маленький для этой суммы. Чтобы его закрыть требуется {RemainingAmount}, а не {amount}.");
    RemainingAmount -= amount;
    if (RemainingAmount == 0) {
      Status = LoanStatus.Closed;
    }
  }
}
