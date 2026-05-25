namespace BankFlow.Domain.Entities;

public class LoanSchedule
{
    public int Id { get; private set; }
    public int LoanId { get; private set; }
    public int InstallmentNumber { get; private set; }
    public DateTime DueDate { get; private set; }
    public decimal PrincipalAmount { get; private set; }
    public decimal InterestAmount { get; private set; }
    public decimal TotalAmount => PrincipalAmount + InterestAmount;
    public decimal Balance { get; private set; }
    public bool IsPaid { get; private set; }
    public DateTime? PaidDate { get; private set; }

    // Navegación
    public Loan Loan { get; private set; } = null!;

    private LoanSchedule() { }

    public LoanSchedule(int loanId, int installmentNumber, DateTime dueDate,
                        decimal principalAmount, decimal interestAmount, decimal balance)
    {
        LoanId = loanId;
        InstallmentNumber = installmentNumber;
        DueDate = dueDate;
        PrincipalAmount = principalAmount;
        InterestAmount = interestAmount;
        Balance = balance;
        IsPaid = false;
    }

    public void MarkAsPaid()
    {
        IsPaid = true;
        PaidDate = DateTime.UtcNow;
    }
}