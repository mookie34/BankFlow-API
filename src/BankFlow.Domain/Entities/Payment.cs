namespace BankFlow.Domain.Entities;

public class Payment
{
    public int Id { get; private set; }
    public int LoanId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal PrincipalAmount { get; private set; }
    public decimal InterestAmount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string? Reference { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navegación: un pago pertenece a un préstamo
    public Loan Loan { get; private set; } = null!;

    private Payment() { }

    public Payment(int loanId, decimal amount, decimal principalAmount,
                   decimal interestAmount, string? reference = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));

        if (principalAmount < 0)
            throw new ArgumentException("Principal amount cannot be negative.", nameof(principalAmount));

        if (interestAmount < 0)
            throw new ArgumentException("Interest amount cannot be negative.", nameof(interestAmount));

        // Validar que capital + intereses = monto total
        var total = principalAmount + interestAmount;
        if (Math.Abs(total - amount) > 0.01m)
            throw new ArgumentException("Principal + Interest must equal the total payment amount.");

        LoanId = loanId;
        Amount = amount;
        PrincipalAmount = principalAmount;
        InterestAmount = interestAmount;
        Reference = reference;
        PaymentDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}