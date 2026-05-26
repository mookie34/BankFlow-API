using BankFlow.Domain.Entities;
using FluentAssertions;

namespace BankFlow.UnitTests.Domain;

public class PaymentTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreatePayment()
    {
        var payment = new Payment(
            loanId: 1,
            amount: 888_489m,
            principalAmount: 788_489m,
            interestAmount: 100_000m,
            reference: "PAY-001"
        );

        payment.LoanId.Should().Be(1);
        payment.Amount.Should().Be(888_489m);
        payment.PrincipalAmount.Should().Be(788_489m);
        payment.InterestAmount.Should().Be(100_000m);
        payment.Reference.Should().Be("PAY-001");
    }

    [Fact]
    public void Constructor_WithZeroAmount_ShouldThrow()
    {
        var act = () => new Payment(1, 0, 0, 0);
        act.Should().Throw<ArgumentException>().WithMessage("*greater than zero*");
    }

    [Fact]
    public void Constructor_WithNegativePrincipal_ShouldThrow()
    {
        var act = () => new Payment(1, 100, -50, 150);
        act.Should().Throw<ArgumentException>().WithMessage("*Principal*");
    }

    [Fact]
    public void Constructor_WithNegativeInterest_ShouldThrow()
    {
        var act = () => new Payment(1, 100, 150, -50);
        act.Should().Throw<ArgumentException>().WithMessage("*Interest*");
    }

    [Fact]
    public void Constructor_WhenPrincipalPlusInterestDoesNotMatchAmount_ShouldThrow()
    {
        // Amount = 1000, pero Principal (600) + Interest (500) = 1100
        var act = () => new Payment(1, 1000m, 600m, 500m);
        act.Should().Throw<ArgumentException>().WithMessage("*Principal + Interest*");
    }

    [Fact]
    public void Constructor_WithNullReference_ShouldAllowIt()
    {
        var payment = new Payment(1, 500m, 400m, 100m);
        payment.Reference.Should().BeNull();
    }

    [Fact]
    public void Constructor_ShouldSetPaymentDateToUtcNow()
    {
        var before = DateTime.UtcNow;
        var payment = new Payment(1, 500m, 400m, 100m);
        var after = DateTime.UtcNow;

        payment.PaymentDate.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }
}