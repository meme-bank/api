using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.ValueObjects;

namespace MembankCore.Domain.Entities.Loan;

public enum LoanStatus {
  Pending,
  Approved,
  Rejected,
  Disbursed, // Пока не используем, может быть потом
  Closed
}

public enum LoanType {
  Defferential,
  Auuetient
}

/// <summary>
/// Класс долга (кредита)
/// </summary>
public class Loan {
  public Guid Id { get; private set; }
  public int BorrowerId { get; private set; } // Кто взял кредит

  public Money PrincipalAmount { get; private set; } // Тело кредита
  public Money RemainingAmount { get; private set; }// Сколько осталось вернуть (с процентами)

  public LoanType Type { get; private set; }

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
    Currency = currency;
  }

  /// <summary>
  /// Добавить долга с процентом. Процент <see cref="InterestRate"/> от <see cref="PrincipalAmount"/> прибаляется к
  /// <see cref="RemainingAmount"/>
  /// </summary>
  /// <param name="daysInYear">Сколько дней в году по РП</param>
  /// <exception cref="InvalidOperationException">Выдаёт исключение если кредит не открыт</exception>
  public void Increment(int daysInYear) {
    if (Status != LoanStatus.Approved)
      throw new InvalidOperationException("Кредит не открыт.");
    Money ofAmount = Type == LoanType.Auuetient ? PrincipalAmount * InterestRate  : RemainingAmount * InterestRate;
    Money dailyDebt = ofAmount / daysInYear;
    RemainingAmount += dailyDebt;
    LastInterestAccrual = DateTime.UtcNow;
  }


  public void Approve() {
    if (Status != LoanStatus.Pending)
      throw new InvalidOperationException("Можно одобрить только ожидающий кредит.");
    UpdateStatus(LoanStatus.Approved);
  }

  public void Reject() {
    if (Status != LoanStatus.Pending)
      throw new InvalidOperationException("Нельзя отклонить уже обработанный кредит.");
    UpdateStatus(LoanStatus.Rejected);
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
