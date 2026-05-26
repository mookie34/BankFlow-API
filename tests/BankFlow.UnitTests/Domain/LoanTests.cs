using BankFlow.Domain.Entities;
using BankFlow.Domain.Enums;
using FluentAssertions;

namespace BankFlow.UnitTests.Domain;

public class LoanTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateLoan()
    {
        var loan = new Loan(
            customerId: 1,
            amount: 10_000_000m,
            interestRate: 12.0m,
            termMonths: 12,
            loanType: LoanType.Personal
        );

        loan.Amount.Should().Be(10_000_000m);
        loan.InterestRate.Should().Be(12.0m);
        loan.TermMonths.Should().Be(12);
        loan.Status.Should().Be(LoanStatus.Pending);
        loan.OutstandingBalance.Should().Be(10_000_000m);
        loan.TotalPaid.Should().Be(0);
        loan.MonthlyPayment.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Constructor_WithZeroAmount_ShouldThrow()
    {
        var act = () => new Loan(1, 0, 12.0m, 12, LoanType.Personal);
        act.Should().Throw<ArgumentException>().WithMessage("*greater than zero*");
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrow()
    {
        var act = () => new Loan(1, -500_000m, 12.0m, 12, LoanType.Personal);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_WithInvalidInterestRate_ShouldThrow(decimal rate)
    {
        var act = () => new Loan(1, 10_000_000m, rate, 12, LoanType.Personal);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(361)]
    public void Constructor_WithInvalidTermMonths_ShouldThrow(int months)
    {
        var act = () => new Loan(1, 10_000_000m, 12.0m, months, LoanType.Personal);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CalculateMonthlyPayment_WithZeroInterest_ShouldDivideEvenly()
    {
        var loan = new Loan(1, 12_000_000m, 0m, 12, LoanType.Personal);

        var payment = loan.CalculateMonthlyPayment();

        payment.Should().Be(1_000_000m);
    }

    [Fact]
    public void CalculateMonthlyPayment_WithInterest_ShouldReturnCorrectAmount()
    {
        // $10M al 12% anual a 12 meses → cuota ≈ $888,488.76
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);

        var payment = loan.CalculateMonthlyPayment();

        payment.Should().BeApproximately(888_488.76m, 1m);
    }

    [Fact]
    public void GenerateAmortizationSchedule_ShouldHaveCorrectNumberOfInstallments()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);

        var schedule = loan.GenerateAmortizationSchedule();

        schedule.Should().HaveCount(12);
        schedule.First().InstallmentNumber.Should().Be(1);
        schedule.Last().InstallmentNumber.Should().Be(12);
    }

    [Fact]
    public void GenerateAmortizationSchedule_LastInstallment_ShouldHaveZeroBalance()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 24, LoanType.Personal);

        var schedule = loan.GenerateAmortizationSchedule();

        schedule.Last().Balance.Should().Be(0);
    }

    [Fact]
    public void GenerateAmortizationSchedule_PrincipalShouldIncreaseOverTime()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);

        var schedule = loan.GenerateAmortizationSchedule();

        // En amortización francesa, el capital aumenta cada mes
        schedule.First().PrincipalAmount.Should().BeLessThan(schedule[6].PrincipalAmount);
    }

    [Fact]
    public void Activate_FromPending_ShouldSetStatusToActive()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);

        loan.Activate();

        loan.Status.Should().Be(LoanStatus.Active);
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldThrow()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);
        loan.Activate();

        var act = () => loan.Activate();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ApplyPayment_ShouldReduceOutstandingBalance()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);
        loan.Activate();

        loan.ApplyPayment(1_000_000m);

        loan.OutstandingBalance.Should().Be(9_000_000m);
        loan.TotalPaid.Should().Be(1_000_000m);
    }

    [Fact]
    public void ApplyPayment_FullBalance_ShouldSetStatusToPaidOff()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);
        loan.Activate();

        loan.ApplyPayment(10_000_000m);

        loan.Status.Should().Be(LoanStatus.PaidOff);
        loan.OutstandingBalance.Should().Be(0);
    }

    [Fact]
    public void ApplyPayment_ExceedingBalance_ShouldThrow()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);
        loan.Activate();

        var act = () => loan.ApplyPayment(15_000_000m);

        act.Should().Throw<ArgumentException>().WithMessage("*exceeds outstanding balance*");
    }

    [Fact]
    public void ApplyPayment_WhenNotActive_ShouldThrow()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);
        // No llamamos Activate(), está en Pending

        var act = () => loan.ApplyPayment(1_000_000m);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelled()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);
        loan.Activate();

        loan.Cancel();

        loan.Status.Should().Be(LoanStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenPaidOff_ShouldThrow()
    {
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);
        loan.Activate();
        loan.ApplyPayment(10_000_000m); // Pagado totalmente

        var act = () => loan.Cancel();

        act.Should().Throw<InvalidOperationException>();
    }
}