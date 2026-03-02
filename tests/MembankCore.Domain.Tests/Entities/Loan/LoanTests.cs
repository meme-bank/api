using MembankCore.Domain.Entities.Loan;
using Xunit;

namespace MembankCore.Domain.Tests.Entities.Loan;

public class LoanTests {
  private MembankCore.Domain.Entities.Loan.Loan CreateTestLoan(decimal principalAmount = 10_000m, decimal interestRate = 0.15m) {
    return new MembankCore.Domain.Entities.Loan.Loan {
      Id = Guid.NewGuid(),
      CurrencyId = "LMC",
      Currency = null!,
      BorrowerId = 1,
      PrincipalAmount = principalAmount,
      RemainingAmount = principalAmount,
      InterestRate = interestRate,
      IssuedAt = DateTime.UtcNow,
      LastInterestAccrual = DateTime.UtcNow,
      Status = LoanStatus.Approved
    };
  }

  [Fact]
  public void Increment_WhenLoanStatusIsApproved() {
    var loan = CreateTestLoan();

    var now = DateTime.UtcNow;
    loan.Increment(now, 1);

    Assert.Equal(11_500m, loan.RemainingAmount);
    Assert.Equal(10_000m, loan.PrincipalAmount);
    Assert.Equal(now, loan.LastInterestAccrual);
  }

  [Fact]
  public void Repay_WhenLoanStatusIsApproved_ButNotClosing() {
    var loan = CreateTestLoan();

    loan.Repay(500m);

    Assert.Equal(9_500m, loan.RemainingAmount);
    Assert.Equal(10_000m, loan.PrincipalAmount);
    Assert.Equal(LoanStatus.Approved, loan.Status);
  }

  [Fact]
  public void Repay_WhenLoanStatusIsApproved_Closing() {
    var loan = CreateTestLoan();

    loan.Repay(10_000m);

    Assert.Equal(0m, loan.RemainingAmount);
    Assert.Equal(10_000m, loan.PrincipalAmount);
    Assert.Equal(LoanStatus.Closed, loan.Status);
  }

  // Asserting throws
  [Theory]
  [InlineData(LoanStatus.Closed)]
  [InlineData(LoanStatus.Pending)]
  [InlineData(LoanStatus.Rejected)]
  [InlineData(LoanStatus.Disbursed)]
  public void Increment_WhenLoanStatusIsNotApproved(LoanStatus status) {
    var loan = CreateTestLoan();
    loan.Status = status;

    Assert.Throws<InvalidOperationException>(() => loan.Increment(DateTime.UtcNow, 12));
  }

  [Theory]
  [InlineData(LoanStatus.Closed)]
  [InlineData(LoanStatus.Pending)]
  [InlineData(LoanStatus.Rejected)]
  [InlineData(LoanStatus.Disbursed)]
  public void Repay_WhenLoanStatusIsNotApproved(LoanStatus status) {
    var loan = CreateTestLoan();
    loan.Status = status;

    Assert.Throws<InvalidOperationException>(() => loan.Repay(10m));
  }

  [Fact]
  public void Repay_WhenLargeAmount() {
    var loan = CreateTestLoan();

    decimal largeAmount = loan.RemainingAmount + 100m;

    Assert.Throws<InvalidOperationException>(() => loan.Repay(largeAmount));
  }

  [Theory]
  [InlineData(-10)]
  [InlineData(-0.01)]
  public void Repay_NegativeAmount(decimal negativeAmount) {
    var loan = CreateTestLoan();

    Assert.Throws<ArgumentException>(() => loan.Repay(negativeAmount));
  }
};
