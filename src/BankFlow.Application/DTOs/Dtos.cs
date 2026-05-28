namespace BankFlow.Application.DTOs
{
    using BankFlow.Domain.Enums;

    public class Dtos
    {
       public record CustomerDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string DocumentNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public int CreditScore { get; init; }
    public int ActiveLoansCount { get; init; }
}

        public record LoanDto
        {
            public int Id { get; init; }
            public int CustomerId { get; init; }
            public string CustomerName { get; init; } = string.Empty;
            public decimal Amount { get; init; }
            public decimal InterestRate { get; init; }
            public int TermMonths { get; init; }
            public string LoanType { get; init; } = string.Empty;
            public string Status { get; init; } = string.Empty;
            public decimal MonthlyPayment { get; init; }
            public decimal TotalPaid { get; init; }
            public decimal OutstandingBalance { get; init; }
            public DateTime StartDate { get; init; }
            public DateTime EndDate { get; init; }
        }

        public record LoanDetailDto
        {
            public int Id { get; init; }
            public int CustomerId { get; init; }
            public string CustomerName { get; init; } = string.Empty;
            public decimal Amount { get; init; }
            public decimal InterestRate { get; init; }
            public int TermMonths { get; init; }
            public string LoanType { get; init; } = string.Empty;
            public string Status { get; init; } = string.Empty;
            public decimal MonthlyPayment { get; init; }
            public decimal TotalPaid { get; init; }
            public decimal OutstandingBalance { get; init; }
            public DateTime StartDate { get; init; }
            public DateTime EndDate { get; init; }
            public List<PaymentDto> Payments { get; init; } = new();
            public List<LoanScheduleDto> Schedule { get; init; } = new();
        }

public record PaymentDto
{
    public int Id { get; init; }
    public int LoanId { get; init; }
    public decimal Amount { get; init; }
    public decimal PrincipalAmount { get; init; }
    public decimal InterestAmount { get; init; }
    public DateTime PaymentDate { get; init; }
    public string? Reference { get; init; }
}

public record LoanScheduleDto
{
    public int InstallmentNumber { get; init; }
    public DateTime DueDate { get; init; }
    public decimal PrincipalAmount { get; init; }
    public decimal InterestAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal Balance { get; init; }
    public bool IsPaid { get; init; }
}

        public record CreateCustomerDto(
            string FirstName,
            string LastName,
            string DocumentNumber,
            string Email,
            string Phone,
            int CreditScore
        );

        public record CreateLoanDto(
            int CustomerId,
            decimal Amount,
            decimal InterestRate,
            int TermMonths,
            LoanType LoanType
        );

        public record CreatePaymentDto(
            int LoanId,
            decimal Amount
        );
    }
}