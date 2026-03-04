using MembankCore.Domain.Entities.Core;
using MembankCore.Domain.Entities.Economic;
using MembankCore.Domain.Entities.Loan;
using MembankCore.Domain.ValueObjects;
using Xunit;

namespace MembankCore.Domain.Tests.Entities.Loan;

public class LoanTests {
  private readonly Currency _testCurrency = new("LMC", "Левро", 1.0m, 1, new Photo(1, PhotoType.Icon));

  private Domain.Entities.Loan.Loan CreateTestLoan(decimal principalAmount = 10_000m, decimal interestRate = 0.15m) {
    // Создаем Money через статический метод или конструктор
    var money = Money.Create(principalAmount, _testCurrency);
    return new Domain.Entities.Loan.Loan(1, money, _testCurrency, interestRate, LoanStatus.Approved);
  }

  [Fact]
  public void Increment_WhenLoanStatusIsApproved() {
    var loan = CreateTestLoan();

    loan.Increment(1);

    // Сравниваем Money с Money или проверяем конкретное поле Amount
    Assert.Equal(11_500m, loan.RemainingAmount.Amount);
    Assert.Equal(10_000m, loan.PrincipalAmount.Amount);
    Assert.Equal(_testCurrency.Id, loan.RemainingAmount.CurrencyId);
    // Assert.Equal(now, loan.LastInterestAccrual); API Changes
  }

  [Fact]
  public void Repay_WhenLoanStatusIsApproved_ButNotClosing() {
    var loan = CreateTestLoan();
    var payment = new Money(500m, _testCurrency.Id);

    loan.Repay(payment);

    Assert.Equal(9_500m, loan.RemainingAmount.Amount);
    Assert.Equal(LoanStatus.Approved, loan.Status);
  }

  [Fact]
  public void Repay_WhenLoanStatusIsApproved_Closing() {
    var loan = CreateTestLoan();
    var payment = loan.RemainingAmount; // Погашаем всю сумму

    loan.Repay(payment);

    Assert.Equal(0m, loan.RemainingAmount.Amount);
    Assert.Equal(LoanStatus.Closed, loan.Status);
  }

  [Theory]
  [InlineData(LoanStatus.Closed)]
  [InlineData(LoanStatus.Pending)]
  [InlineData(LoanStatus.Rejected)]
  [InlineData(LoanStatus.Disbursed)]
  public void Increment_WhenLoanStatusIsNotApproved(LoanStatus status) {
    var loan = CreateTestLoan();
    loan.UpdateStatus(status);

    Assert.Throws<InvalidOperationException>(() => loan.Increment(12));
  }

  [Theory]
  [InlineData(LoanStatus.Closed)]
  [InlineData(LoanStatus.Pending)]
  [InlineData(LoanStatus.Rejected)]
  [InlineData(LoanStatus.Disbursed)]
  public void Repay_WhenLoanStatusIsNotApproved(LoanStatus status) {
    var loan = CreateTestLoan();
    loan.UpdateStatus(status);
    var payment = new Money(10m, _testCurrency.Id);

    Assert.Throws<InvalidOperationException>(() => loan.Repay(payment));
  }

  [Fact]
  public void Repay_WhenLargeAmount() {
    var loan = CreateTestLoan();
    var largeAmount = loan.RemainingAmount + new Money(100m, _testCurrency.Id);

    Assert.Throws<ArgumentException>(() => loan.Repay(largeAmount));
  }

  [Theory]
  [InlineData(-10)]
  [InlineData(-0.01)]
  public void Repay_NegativeAmount(decimal negativeAmount) {
    var loan = CreateTestLoan();
    var payment = new Money(negativeAmount, _testCurrency.Id);

    Assert.Throws<ArgumentException>(() => loan.Repay(payment));
  }

  [Fact]
  public void Repay_WithWrongCurrency_ShouldThrow() {
    var loan = CreateTestLoan();
    var wrongMoney = new Money(100m, "USD");

    Assert.Throws<ArgumentException>(() => loan.Repay(wrongMoney));
  }
}
